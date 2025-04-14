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
        private readonly IUserChallengeProgressRepository _challengeProgressRepository;


        private const double FIXED_LONGITUDE = 7.9652276;
        private const double FIXED_LATITUDE = 58.1358112;

        public TransportEntryController(ITransportEntryRepository transportEntryRepository, IUserRepository userRepository,
         IAchievementRepository achievementRepository, 
         IServiceProvider serviceProvider,
         IGeocodingService geocodingService,
         IDistanceService distanceService,
         ChallengeProgressService challengeProgressService,
         IUserChallengeProgressRepository challengeProgressRepository)
        {
            _transportEntryRepository = transportEntryRepository;
            _userRepository = userRepository;
            _achievementRepository = achievementRepository;
            _serviceProvider = serviceProvider;
            _geocodingService = geocodingService;
            _distanceService = distanceService;
            _challengeProgressService = challengeProgressService;
            _challengeProgressRepository = challengeProgressRepository;
        }

    [HttpPost("upsert")]
    public async Task<IActionResult> Post([FromBody] TransportEntryModel model)
    {
        try
        {
            var email = User.FindFirst("emails")?.Value;
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email claim missing.");

            if (model == null || string.IsNullOrEmpty(model.StartingAddress))
                return BadRequest("Invalid data.");

            var user = _userRepository.GetUserByEmail(email);
            if (user == null)
                return NotFound("User not found.");

            var startingCoordinates = await _geocodingService.GetCoordinates(model.StartingAddress);
            if (startingCoordinates == null)
                return BadRequest("Could not geocode the starting address.");

            var fixedDestination = new double[] { FIXED_LONGITUDE, FIXED_LATITUDE };
            double distanceKm = await _distanceService.GetDistance(startingCoordinates, fixedDestination);
            double co2utslipp = CalculateCo2(distanceKm, model.Method);
            int calculatedPoints = CalculatePoints(distanceKm, model.Method);
            double calculatedMoney = CalculateMoneySaved(distanceKm, model.Method);

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

            var entries = _transportEntryRepository.GetEntriesForUser(user.UserId);
            var attempts = _challengeProgressRepository.GetAttemptsByUserId(user.UserId);

            await _achievementRepository.CheckForAchievements(user.UserId, model.Method, entries, attempts, entity);
            await _challengeProgressService.CheckAndUpdateProgress(user.UserId, entity.Method, entries, attempts);

            return Ok(new
            {
                message = "Data received successfully.",
                distanceKm,
                calculatedPoints
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Transport upsert failed: " + ex.Message);
            return StatusCode(503, new
            {
                message = "Klarte ikke å beregne reise akkurat nå.",
                detail = ex.Message
            });
        }
    }


        private int CalculatePoints(double distanceKm, Methode mode)
        {
            // CO₂ (grams) per km for each mode.
            // car is carpooling so divided by 2
            const double singleOccupantCO2 = 120.0;  // baseline for comparison
            const double carpoolCO2 = 60.0;          // carpooling effective emission
            const double busCO2 = 70.0;
            const double cycleCO2 = 0.0;
            const double walkCO2 = 0.0;

            double modeCO2;
            switch (mode)
            {
                case Methode.Car:
                    // Car here represents carpooling.
                    modeCO2 = carpoolCO2;
                    break;
                case Methode.Bus:
                    modeCO2 = busCO2;
                    break;
                case Methode.Cycling:
                    modeCO2 = cycleCO2;
                    break;
                case Methode.Walking:
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
            switch (mode)
            {
                case Methode.Bus:
                    maxCap = 300;
                    break;
                case Methode.Cycling:
                case Methode.Walking:
                    maxCap = 500;
                    break;
                case Methode.Car:
                default:
                    maxCap = 200;
                    break;
            }

            // Apply the cap.
            final = Math.Min(final, maxCap);

            return (int)Math.Round(final);
        }

        private double CalculateCo2(double distanceKm, Methode mode)
        {
            
        var emissionFactors = new Dictionary<Methode, double>()
        {
            { Methode.Car, 0.2 },      
            { Methode.Bus, 0.1 },      
            { Methode.Cycling, 0.0 },  
            { Methode.Walking, 0.0 }   
        };
        
        double defaultEmission = 0.2;
        double selectedEmission = emissionFactors.GetValueOrDefault(mode, defaultEmission);

        // Calculate saved CO2 by comparing with car usage
        double savedCo2 = (defaultEmission - selectedEmission) * distanceKm;
        return Math.Max(savedCo2, 0); // Ensure no negative values
        }

        private double CalculateMoneySaved(double distanceKm, Methode mode)
        {
            var costPerKm = new Dictionary<Methode, double>()
            {
                { Methode.Car, 5.00 },      // 5 kr per km (drivstoff, vedlikehold, verdifall)
                { Methode.Bus, 1.19 },      // Basert på 711 kr/mnd og 600 km gjennomsnitt
                { Methode.Cycling, 0.00 },  
                { Methode.Walking, 0.00 }   
            };

            double defaultCost = 5.00; // Standard kostnad (bil)
            double selectedCost = costPerKm.GetValueOrDefault(mode, defaultCost);

            // Beregn spart penger sammenlignet med bil
            double savedMoney = (defaultCost - selectedCost) * distanceKm;
            return Math.Max(savedMoney, 0);
        }

        [HttpGet("missing-today")]
        [AllowAnonymous] 
        public IActionResult GetUsersMissingToday()
        {
            try
            {
                var allUsers = _userRepository.GetAllUsers(); 
                var today = DateTime.UtcNow.Date;

                var missingUsers = allUsers
                    .Where(user =>
                        !_transportEntryRepository
                            .GetEntriesForUser(user.UserId)
                            .Any(e => e.CreatedAt.Date == today))
                    .Select(user => new {
                        user.UserId,
                        user.Email
                    })
                    .ToList();

                return Ok(missingUsers);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Feil ved henting av manglende brukere: " + ex.Message);
                return StatusCode(500, new { message = "Klarte ikke hente data." });
            }
        }
    }
}