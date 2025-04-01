using System;
using Microsoft.AspNetCore.Mvc;
using BouvetBackend.Models.TransportEntryModel;
using BouvetBackend.Entities;
using BouvetBackend.Repositories;
using BouvetBackend.Services;
using Microsoft.AspNetCore.Authorization;

namespace BouvetBackend.Controllers 
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Require authentication
    public class TransportEntryController : Controller
    {
        private readonly ITransportEntryRepository _transportEntryRepository;
        private readonly IUserRepository _userRepository; 
        private readonly IAchievementRepository _achievementRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly IGeocodingService _geocodingService;
        private readonly IDistanceService _distanceService;
        private readonly ChallengeProgressService _challengeProgressService;


        private const double FIXED_LONGITUDE = 7.9652276;
        private const double FIXED_LATITUDE = 58.1358112;

        public TransportEntryController(ITransportEntryRepository transportEntryRepository, IUserRepository userRepository,
         IAchievementRepository achievementRepository, 
         IServiceProvider serviceProvider,
         IGeocodingService geocodingService,
         IDistanceService distanceService,
         ChallengeProgressService challengeProgressService)
        {
            _transportEntryRepository = transportEntryRepository;
            _userRepository = userRepository;
            _achievementRepository = achievementRepository;
            _serviceProvider = serviceProvider;
            _geocodingService = geocodingService;
            _distanceService = distanceService;
            _challengeProgressService = challengeProgressService;


        }

        [HttpPost("upsert")]
public async Task<IActionResult> Post([FromBody] TransportEntryModel model)
{
    try
    {
        var email = User.FindFirst("emails")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email claim missing.");
            }

            if (model == null || string.IsNullOrEmpty(model.StartingAddress))
            {
                return BadRequest("Invalid data.");
            }

            var user = _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // The address of the user.
            var startingCoordinates = await _geocodingService.GetCoordinates(model.StartingAddress);
            if (startingCoordinates == null)
            {
                return BadRequest("Could not geocode the starting address.");
            }

            // The fixed adress. Set to Hennig Olsen Is.
            var fixedDestination = new double[] { FIXED_LONGITUDE, FIXED_LATITUDE };

            double distanceKm = await _distanceService.GetDistance(startingCoordinates, fixedDestination);
            
            // Calculate co2 saved for this submission.
            double co2utslipp = CalculateCo2(distanceKm, model.Method ?? string.Empty);

            // Calculate points based on the distance.
            int calculatedPoints = CalculatePoints(distanceKm, model.Method ?? string.Empty);
            
            // Calculate saved Money based on the distance.
            double calculatedMoney = CalculateMoneySaved(distanceKm, model.Method ?? string.Empty);

            var entity = new TransportEntry
            {
                UserId = user.UserId, 
                Method = model.Method,
                Points = calculatedPoints,
                Co2 = co2utslipp,
                DistanceKm = distanceKm,
                MoneySaved = calculatedMoney,
                CreatedAt = DateTime.UtcNow
            };

            _transportEntryRepository.Upsert(entity);

            await _achievementRepository.CheckForAchievements(user.UserId, model.Method ?? string.Empty, entity);
            await _challengeProgressService.CheckAndUpdateProgress(user.UserId, entity.Method);

            return Ok(new 
            { 
                message = "Data received successfully.", 
                distanceKm, 
                calculatedPoints 
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal error: {ex.Message}");
        }
    }

        private int CalculatePoints(double distanceKm, string mode)
        {
            // CO₂ (grams) per km for each mode.
            // car is carpooling so divided by 2
            const double singleOccupantCO2 = 120.0;  // baseline for comparison
            const double carpoolCO2 = 60.0;          // carpooling effective emission
            const double busCO2 = 70.0;
            const double cycleCO2 = 0.0;
            const double walkCO2 = 0.0;

            double modeCO2;
            switch (mode.ToLower())
            {
                case "car":
                    // Car here represents carpooling.
                    modeCO2 = carpoolCO2;
                    break;
                case "bus":
                    modeCO2 = busCO2;
                    break;
                case "cycling":
                    modeCO2 = cycleCO2;
                    break;
                case "walking":
                    modeCO2 = walkCO2;
                    break;
                default:
                    modeCO2 = singleOccupantCO2;
                    break;
            }

            // Exponent is multiplier upwards
            double exponent = 0.5;
            double adjustedDistance = Math.Pow(distanceKm, exponent);

            // Multiply the baseline CO2 by the adjusted distance to get score.
            double baselineCO2 = singleOccupantCO2 * adjustedDistance;
            double actualCO2 = modeCO2 * adjustedDistance;
            double co2Saved = baselineCO2 - actualCO2;
            if (co2Saved < 0)
            {
                co2Saved = 0;
            }

            // Convert grams CO2 saved into points.
            // First, calculate a raw point value.
            double points = co2Saved / 50.0;
            double final = points * 50; // scaling factor

            // Define max caps per mode.
            double maxCap;
            switch (mode.ToLower())
            {
                case "bus":
                    maxCap = 300;
                    break;
                case "cycling":
                case "walking":
                    maxCap = 500;
                    break;
                case "car":
                default:
                    maxCap = 200;
                    break;
            }

            // Apply the cap.
            final = Math.Min(final, maxCap);

            return (int)Math.Round(final);
        }

        private double CalculateCo2(double distanceKm, string mode)
        {
            
        var emissionFactors = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
        {
            { "car", 0.2 },      
            { "bus", 0.1 },      
            { "cycling", 0.0 },  
            { "walking", 0.0 }   
        };
        
        double defaultEmission = 0.2;
        double selectedEmission = emissionFactors.GetValueOrDefault(mode, defaultEmission);

        // Calculate saved CO2 by comparing with car usage
        double savedCo2 = (defaultEmission - selectedEmission) * distanceKm;
        return Math.Max(savedCo2, 0); // Ensure no negative values
        }

        private double CalculateMoneySaved(double distanceKm, string mode)
        {
            var costPerKm = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                { "car", 5.00 },      // 5 kr per km (drivstoff, vedlikehold, verdifall)
                { "bus", 1.19 },      // Basert på 711 kr/mnd og 600 km gjennomsnitt
                { "cycling", 0.00 },  
                { "walking", 0.00 }   
            };

            double defaultCost = 5.00; // Standard kostnad (bil)
            double selectedCost = costPerKm.GetValueOrDefault(mode, defaultCost);

            // Beregn spart penger sammenlignet med bil
            double savedMoney = (defaultCost - selectedCost) * distanceKm;
            return Math.Max(savedMoney, 0);
        }


    }
}
