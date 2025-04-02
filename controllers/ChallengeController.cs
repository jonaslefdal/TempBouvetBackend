using System;
using Microsoft.AspNetCore.Mvc;
using BouvetBackend.Entities;
using BouvetBackend.Models.ChallengeModel;
using BouvetBackend.Repositories;
using BouvetBackend.Models.UserModel;
using BouvetBackend.Models.CompleteChallengeModel;
using Microsoft.AspNetCore.Authorization;
using BouvetBackend.Extensions;
using System.Diagnostics;


namespace BouvetBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChallengeController : Controller
    {
        private readonly IChallengeRepository _challengeRepository;
        private readonly IUserChallengeProgressRepository _challengeProgressRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITransportEntryRepository _transportRepository;
        private readonly IAchievementRepository _achievementRepository;
        private readonly IServiceProvider _serviceProvider;

        public ChallengeController(IChallengeRepository challengeRepository,
                                   IUserChallengeProgressRepository challengeProgressRepository,
                                   IUserRepository userRepository,
                                   ITransportEntryRepository transportRepository,
                                   IAchievementRepository achievementRepository,
                                   IServiceProvider serviceProvider)
        {
            _challengeRepository = challengeRepository;
            _challengeProgressRepository = challengeProgressRepository;
            _userRepository = userRepository;
            _transportRepository = transportRepository;
            _achievementRepository = achievementRepository;
            _serviceProvider = serviceProvider;  

        }

        // GET: api/challenge
        [HttpGet]
        public IActionResult GetAllChallenges()
        {
            var challenges = _challengeRepository.GetAll();
            return Ok(challenges);
        }
        [HttpGet("current")]
        public IActionResult GetCurrentChallenges([FromQuery] DateTime? testDate)
        {
            // Use testDate if provided, otherwise default to now.
            DateTime now = testDate ?? DateTime.UtcNow;
            
            int totalGroups = 10;
            // cycleStart = startDate
            DateTime cycleStart = new DateTime(2025, 3, 13);
            int weeksSinceStart = (int)((now - cycleStart).TotalDays / 7);
            int currentGroup = (weeksSinceStart % totalGroups) + 1;
            
            var challenges = _challengeRepository.GetChallengesByRotationGroup(currentGroup);
            
            if (challenges == null || challenges.Count == 0)
            {
                return NotFound("No challenges found for the current group.");
            }
            
            return Ok(challenges);
        }

        // GET: api/challenge/user
        [HttpGet("user")]
        public IActionResult GetUserChallenges() 
        {
            var email = User.FindFirst("emails")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email parameter is required.");
            }

            var user = _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var attempts = _challengeProgressRepository.GetAttemptsByUserId(user.UserId);

             var grouped = attempts
            .GroupBy(a => a.ChallengeId)
            .Select(g => new 
            {
                ChallengeId = g.Key, 
                AttemptCount = g.Count() 
            })
            .ToList();

            return Ok(grouped);
        }

        // POST: api/challenge/complete
        [HttpPost("complete")]
        public IActionResult CompleteChallenge([FromBody] CompleteChallengeRequest request)
        {
            var email = User.FindFirst("emails")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email parameter is required.");
            }

            if (request == null)
            {
                return BadRequest("Invalid data.");
            }

            var user = _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var challenge = _challengeRepository.Get(request.ChallengeId);
            if (challenge == null)
            {
                return NotFound("Challenge not found.");
            }

            // Get current attempt count for this challenge.
            var attemptCount = _challengeProgressRepository
                .GetAttemptsByUserForChallenge(user.UserId, challenge.ChallengeId)
                .Count;

            if (attemptCount >= challenge.MaxAttempts)
            {
                return BadRequest("Maximum attempts reached for this challenge.");
            }

            // Determine if this is the final attempt.
            bool isFinalAttempt = (attemptCount + 1 == challenge.MaxAttempts);

            // Create a new challenge attempt with points based on final attempt status.
            var attempt = new UserChallengeProgress
            {
                UserId = user.UserId,
                ChallengeId = challenge.ChallengeId,
                PointsAwarded = isFinalAttempt ? challenge.Points : 0,
                AttemptedAt = DateTime.UtcNow
            };

            // Repository will update the user's TotalScore.
            _challengeProgressRepository.Upsert(attempt);

            return Ok(new
            {
                message = isFinalAttempt
                    ? "Challenge fully completed! Points awarded."
                    : null,
                attemptId = attempt.UserChallengeProgressId
            });
        }


        [HttpGet("current/user")]
        public async Task<IActionResult> GetCurrentUserChallenges([FromQuery] DateTime? testDate)
        {
            var email = User.FindFirst("emails")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email parameter is required.");
            }

            var user = _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            DateTime now = testDate ?? DateTime.UtcNow;

            int totalGroups = 10;
            DateTime cycleStart = new DateTime(2025, 3, 13);
            int weeksSinceStart = (int)((now - cycleStart).TotalDays / 7);
            int currentGroup = (weeksSinceStart % totalGroups) + 1;

            var currentChallenges = _challengeRepository.GetChallengesByRotationGroup(currentGroup);
            var startOfWeek = now.StartOfWeek(DayOfWeek.Monday);

            var result = new List<object>();

            foreach (var challenge in currentChallenges)
            {
                double userProgress;

                if (challenge.RequiredTransportMethod == Methode.Custom)
                {
                    userProgress = await _challengeProgressRepository
                        .GetUserAttemptCountForChallengeThisWeekAsync(user.UserId, challenge.ChallengeId, startOfWeek);
                }
                else if (challenge.ConditionType == "Distance" && challenge.RequiredDistanceKm.HasValue)
                {
                    userProgress = _transportRepository.GetTransportDistanceSum(
                        user.UserId, challenge.RequiredTransportMethod, startOfWeek);
                }
                else
                {
                    userProgress = _transportRepository.GetTransportEntryCount(
                        user.UserId, challenge.RequiredTransportMethod, startOfWeek);
                }

                result.Add(new
                {
                    ChallengeId = challenge.ChallengeId,
                    Description = challenge.Description,
                    Method = challenge.ConditionType == "Custom" 
                        ? "custom" 
                        : challenge.RequiredTransportMethod.ToString().ToLower(),
                    Points = challenge.Points,
                    Type = challenge.ConditionType,
                    RequiredCount = challenge.MaxAttempts,
                    RequiredDistanceKm = challenge.RequiredDistanceKm,
                    UserProgress = userProgress
                });
            }

            return Ok(result);
        }


        [HttpPost("custom/complete")]
        public async Task<IActionResult> CompleteCustomChallenge([FromBody] CompleteChallengeRequest request)
        {

            var email = User.FindFirst("emails")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email parameter is required.");
            }

            if (request == null || request.ChallengeId <= 0)
            {
                return BadRequest("Invalid data.");
            }

            var user = _userRepository.GetUserByEmail(email);
            if (user == null)
                return NotFound("User not found.");

            var challenge = _challengeRepository.Get(request.ChallengeId);
            if (challenge == null || challenge.RequiredTransportMethod != Methode.Custom)
                return BadRequest("Invalid or non-custom challenge.");

            DateTime weekStart = DateTime.UtcNow.StartOfWeek(DayOfWeek.Monday);

            int attemptsThisWeek = await _challengeProgressRepository
                .GetUserAttemptCountForChallengeThisWeekAsync(user.UserId, challenge.ChallengeId, weekStart);


            if (attemptsThisWeek >= challenge.MaxAttempts)
                return BadRequest("Challenge already completed for this week.");

            // Record the attempt
            var attempt = new UserChallengeProgress
            {
                UserId = user.UserId,
                ChallengeId = challenge.ChallengeId,
                PointsAwarded = (attemptsThisWeek + 1 == challenge.MaxAttempts) ? challenge.Points : 0,
                AttemptedAt = DateTime.UtcNow
            };

            _challengeProgressRepository.Upsert(attempt); 

            if (attempt.PointsAwarded > 0)
            {
                user.TotalScore += attempt.PointsAwarded;
            } 

            if (attempt.PointsAwarded > 0)
            {
                var entries = _transportRepository.GetEntriesForUser(user.UserId);
                var userChallenges = _challengeProgressRepository.GetAttemptsByUserId(user.UserId);

                await _achievementRepository.CheckForAchievements(
                    user.UserId,
                    challenge.RequiredTransportMethod,
                    entries,
                    userChallenges
                );
            }

            return Ok(new
            {
                completed = (attemptsThisWeek + 1 == challenge.MaxAttempts),
                pointsAwarded = attempt.PointsAwarded,
                message = (attempt.PointsAwarded > 0) ? "Challenge completed!" : "Progress recorded!",
                attemptId = attempt.UserChallengeProgressId
            });
        }
    }
}
