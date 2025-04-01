using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BouvetBackend.Models.UserModel;
using BouvetBackend.Entities;
using BouvetBackend.Repositories;
using BouvetBackend.DTO;

namespace BouvetBackend.Controllers 
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Require authentication
    public class ProfileController : Controller
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITransportEntryRepository _transportEntryRepository;
        private readonly IChallengeRepository _challengeRepository;
        private readonly IUserChallengeProgressRepository _challengeProgressRepository;
        private readonly IAchievementRepository _achievementRepository;

        public ProfileController(ICompanyRepository companyRepository, 
        IUserRepository userRepository, 
        ITransportEntryRepository transportEntryRepository,
        IChallengeRepository challengeRepository,
        IUserChallengeProgressRepository challengeProgressRepository,
        IAchievementRepository achievementRepository
        )
        {
            _companyRepository = companyRepository;
            _userRepository = userRepository;
            _transportEntryRepository = transportEntryRepository;
            _challengeRepository = challengeRepository;
            _challengeProgressRepository = challengeProgressRepository;
            _achievementRepository = achievementRepository;
        }

        // All api calls togheter to save load times 
        [HttpGet("overview")]
        public IActionResult GetProfileOverview()
        {
            var email = User.FindFirst("emails")?.Value;
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email claim missing.");

            var user = _userRepository.GetUserByEmail(email);
            if (user == null)
                return NotFound("User not found.");

            // Fetch profile related data
            double totalCo2Savings = _transportEntryRepository.GetTotalCo2SavingsByUser(user.UserId);
            int totalTravels = _transportEntryRepository.GetTotalTravelCountByUser(user.UserId);
            double totalMoneySaved = _transportEntryRepository.GetTotalMoneySaved(user.UserId);

            // Calculate completed challenges
            var attempts = _challengeProgressRepository.GetAttemptsByUserId(user.UserId);
            int completedChallenges = attempts
                .Select(a => a.ChallengeId)
                .Distinct()
                .Count();

            // Fetch achievements data
            var achievements = _achievementRepository.GetAll();
            var earned = _achievementRepository.GetUserAchievements(user.UserId);
            var progressMap = _achievementRepository.GetAchievementProgress(user.UserId);

            // Map achievements into AchievementDto list
            var achievementsDto = achievements.Select(a => new AchievementDto
            {
                AchievementId = a.AchievementId,
                Name = a.Name,
                Description = a.Description,
                Total = a.Threshold,
                Progress = progressMap.GetValueOrDefault(a.AchievementId, 0),
                EarnedAt = earned.FirstOrDefault(e => e.AchievementId == a.AchievementId)?.EarnedAt
            }).ToList();

            var userModel = new UserModel
            {
                Name = user.Name,
                NickName = user.NickName,
                TotalScore = user.TotalScore,
                ProfilePicture = user.ProfilePicture
            };

            var profileOverviewDto = new ProfileOverviewDto
            {
                User = userModel,
                TotalCo2Savings = totalCo2Savings,
                TotalTravels = totalTravels,
                TotalMoneySaved = totalMoneySaved,
                CompletedChallenges = completedChallenges,
                Achievements = achievementsDto
            };

            return Ok(profileOverviewDto);

        }

        [HttpGet("allComp")]
        public IActionResult GetAllCompanies()
        {
            var company = _companyRepository.GetAll();
            if (company == null || company.Count == 0)
            {
                return NotFound("No companies found.");
            }

            return Ok(company);
        }

        [HttpGet("getUser")]
        public IActionResult GetMyProfile()
        {
            // Assume the user's email is in the token
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
            
            return Ok(user);
        }

        [HttpPut("companySet")]
        public IActionResult SetUserCompany([FromBody] UserModel userModel)
        {
            if (userModel == null)
            {
                return BadRequest("Invalid request data.");
            }
            
            var email = User.FindFirst("emails")?.Value;
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email claim missing.");

            var user = _userRepository.GetUserByEmail(email);
            if (user == null)
                return NotFound("User not found.");

            // Update the user's company
            user.CompanyId = userModel.CompanyId;
            user.NickName = userModel.NickName; 
            user.Address = userModel.Address;

            _userRepository.InsertOrUpdateUser(user);

            return Ok(new { message = "User company updated successfully." });
        }

        [HttpGet("totalCo2")]
        public IActionResult GetTotalCo2Savings()
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

            double totalCo2Savings = _transportEntryRepository.GetTotalCo2SavingsByUser(user.UserId);

            return Ok(new { totalCo2Savings });
        }

        [HttpGet("totalTravels")]
        public IActionResult GetTotalTravels()
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

        int totalTravels = _transportEntryRepository.GetTotalTravelCountByUser(user.UserId);

        return Ok(new { totalTravels });
        }

        [HttpGet("totalMoney")]
        public IActionResult GetTotalMoney()
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

        double totalMoneySaved = _transportEntryRepository.GetTotalMoneySaved(user.UserId);

        return Ok(new { totalMoneySaved });
        }

        [HttpGet("completedcount")]
        public IActionResult GetCompletedChallengesCount()
        {
            var email = User.FindFirst("emails")?.Value;
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email parameter is required.");

            var user = _userRepository.GetUserByEmail(email);
            if (user == null)
                return NotFound("User not found.");

            // Get all attempts for this user
            var attempts = _challengeProgressRepository.GetAttemptsByUserId(user.UserId);

            // Count how many are completed
            int completedCount = attempts
                .Select(a => a.ChallengeId)
                .Distinct()
                .Count();

            // Return just the count
            return Ok(new { completedChallenges = completedCount });
        }

    }
}
