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

        public AchievementFuncController(IAchievementRepository achievementRepository, IUserRepository userRepository)
        {
            _achievementRepository = achievementRepository;
            _userRepository = userRepository;
        }

        [HttpGet("getUserAchivements")]
        public IActionResult GetUserAchivements()
        {
            var email = User.FindFirst("emails")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email claim missing.");
            }
            
            var user = _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            var data = _achievementRepository.GetUserAchievements(user.UserId);
            
            return Ok(data);
        }
    }
}
