using System.Threading.Tasks;
using BouvetBackend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BouvetBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Require authentication
    public class AchievementFuncController : ControllerBase
    {
        private readonly IAchievementRepository _achievementRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserChallengeProgressRepository _challengeProgressRepository;
        private readonly ITransportEntryRepository _transportEntryRepository;

        public AchievementFuncController(IAchievementRepository achievementRepository, 
        IUserRepository userRepository, 
        IUserChallengeProgressRepository challengeProgressRepository,
        ITransportEntryRepository transportEntryRepository)    
        {
            _achievementRepository = achievementRepository;
            _userRepository = userRepository;
            _challengeProgressRepository = challengeProgressRepository; 
            _transportEntryRepository = transportEntryRepository;  
        }

        [HttpGet("getUserAchievements")]
        public async Task<IActionResult> GetUserAchievements()
        {
            var email = User.FindFirst("emails")?.Value;
            if (string.IsNullOrEmpty(email)) return BadRequest("Email claim missing.");

            var user = _userRepository.GetUserByEmail(email);
            if (user == null) return NotFound("User not found.");

            var achievements = _achievementRepository.GetAll(); // total thresholds
            var earned = _achievementRepository.GetUserAchievements(user.UserId);
            var entries = _transportEntryRepository.GetEntriesForUser(user.UserId);
            var attempts = _challengeProgressRepository.GetAttemptsByUserId(user.UserId);
            var progressMap = await _achievementRepository
                .GetAchievementProgress(user.UserId, entries, attempts);


            var result = achievements.Select(a =>
            {
                var earnedEntry = earned.FirstOrDefault(e => e.AchievementId == a.AchievementId);
                return new {
                    achievementId = a.AchievementId,
                    name = a.Name,
                    description = a.Description,
                    total = a.Threshold,
                    progress = progressMap.GetValueOrDefault(a.AchievementId, 0),
                    earnedAt = earnedEntry?.EarnedAt
                };
            });

            return Ok(result);
        }


    }
}
