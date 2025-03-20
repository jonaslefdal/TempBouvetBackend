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


        private const double FIXED_LONGITUDE = 7.9652276;
        private const double FIXED_LATITUDE = 58.1358112;

        public TransportEntryController(ITransportEntryRepository transportEntryRepository, IUserRepository userRepository,
         IAchievementRepository achievementRepository, 
         IServiceProvider serviceProvider,
         IGeocodingService geocodingService,
         IDistanceService distanceService)
        {
            _transportEntryRepository = transportEntryRepository;
            _userRepository = userRepository;
            _achievementRepository = achievementRepository;
            _serviceProvider = serviceProvider;
            _geocodingService = geocodingService;
            _distanceService = distanceService;


        }

        [HttpPost("upsert")]
        public async Task<IActionResult> Post([FromBody] TransportEntryModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.StartingAddress))
            {
                return BadRequest("Invalid data.");
            }

            var user = _userRepository.GetUserByEmail(model.Email ?? "");
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

            // Fire-and-forget achievement checking.
            _ = Task.Run(async () =>
            {
                using var scope = _serviceProvider.CreateScope();
                var achievementRepository = scope.ServiceProvider.GetRequiredService<IAchievementRepository>();
                await achievementRepository.CheckForAchievements(user.UserId, model.Method ?? string.Empty);
            });

            return Ok(new 
            { 
                message = "Data received successfully.", 
                distanceKm, 
                calculatedPoints 
            });
        }

        private int CalculatePoints(double distanceKm, string mode)
        {
            double baseMultiplier = 5; // internal base
            double exponent = 0.9;     // sub-linear exponent
            double modeMultiplier = mode.ToLower() switch
            {
                "car"     => 1.00,
                "bus"     => 1.05,
                "cycling" => 1.08,
                "walking" => 1.15,
                _         => 1.00
            };

            // Sub-linear growth: distance^exponent
            double rawScore = baseMultiplier * Math.Pow(distanceKm, exponent) * modeMultiplier;

            // Multiply by 10 to get a larger final value
            double finalScore = rawScore * 10.0;

            return (int)finalScore;
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
