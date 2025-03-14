using System;
using Microsoft.AspNetCore.Mvc;
using BouvetBackend.Entities;
using BouvetBackend.Models.ChallengeModel;
using BouvetBackend.Repositories;
using BouvetBackend.Models.UserModel;
using BouvetBackend.Models.CompleteChallengeModel;
using Microsoft.AspNetCore.Authorization;

namespace BouvetBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChallengeController : Controller
    {
        private readonly IChallengeRepository _challengeRepository;
        private readonly IUserChallengeAttemptRepository _challengeAttemptRepository;
        private readonly IUserRepository _userRepository;
        private readonly IWeeklyChallengeRepository _weeklyChallengeRepository;


        public ChallengeController(IChallengeRepository challengeRepository,
                                   IUserChallengeAttemptRepository challengeAttemptRepository,
                                   IUserRepository userRepository,
                                   IWeeklyChallengeRepository weeklyChallengeRepository)
        {
            _challengeRepository = challengeRepository;
            _challengeAttemptRepository = challengeAttemptRepository;
            _userRepository = userRepository;
            _weeklyChallengeRepository = weeklyChallengeRepository;

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

            var attempts = _challengeAttemptRepository.GetAttemptsByUserId(user.UserId);

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
        if (request == null)
        {
            return BadRequest("Invalid data.");
        }

        var user = _userRepository.GetUserByEmail(request.Email);
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
        var attemptCount = _challengeAttemptRepository
            .GetAttemptsByUserForChallenge(user.UserId, challenge.ChallengeId)
            .Count;

        if (attemptCount >= challenge.MaxAttempts)
        {
            return BadRequest("Maximum attempts reached for this challenge.");
        }

        // Determine if this is the final attempt.
        bool isFinalAttempt = (attemptCount + 1 == challenge.MaxAttempts);

        // Create a new challenge attempt with points based on final attempt status.
        var attempt = new UserChallengeAttempt
        {
            UserId = user.UserId,
            ChallengeId = challenge.ChallengeId,
            PointsAwarded = isFinalAttempt ? challenge.Points : 0,
            AttemptedAt = DateTime.UtcNow
        };

        // Repository will update the user's TotalScore.
        _challengeAttemptRepository.Upsert(attempt);

        return Ok(new
        {
            message = isFinalAttempt
                ? "Challenge fully completed! Points awarded."
                : null,
            attemptId = attempt.UserChallengeAttemptId
        });
    }




    }
}
