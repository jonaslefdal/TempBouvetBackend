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
        private readonly IEndUserAddressRepository _endUserAddressRepository;

        public ProfileController(ICompanyRepository companyRepository, 
        IUserRepository userRepository, 
        ITransportEntryRepository transportEntryRepository,
        IChallengeRepository challengeRepository,
        IUserChallengeProgressRepository challengeProgressRepository,
        IAchievementRepository achievementRepository,
        IEndUserAddressRepository endUserAddressRepository
        )
        {
            _companyRepository = companyRepository;
            _userRepository = userRepository;
            _transportEntryRepository = transportEntryRepository;
            _challengeRepository = challengeRepository;
            _challengeProgressRepository = challengeProgressRepository;
            _achievementRepository = achievementRepository;
            _endUserAddressRepository = endUserAddressRepository;
        }

        // All api calls togheter to save load times 
        [HttpGet("overview")]
        public async Task<IActionResult> GetProfileOverview()
        {
            var email = User.FindFirst("emails")?.Value;
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email claim missing.");

            var user = _userRepository.GetUserByEmail(email);
            if (user == null)
                return NotFound("User not found.");

            // Fetch profile related data
            var entries = _transportEntryRepository.GetEntriesForUser(user.UserId);
            double totalCo2Savings = entries.Sum(te => te.Co2);
            int totalTravels = entries.Count;
            double totalMoneySaved = entries.Sum(te => te.MoneySaved);

            // Calculate completed challenges
            var attempts = _challengeProgressRepository.GetAttemptsByUserId(user.UserId);
            int completedChallenges = attempts
                .Select(a => a.ChallengeId)
                .Distinct()
                .Count();

            // Fetch achievements data
            var achievements = _achievementRepository.GetAll();
            var earned = _achievementRepository.GetUserAchievements(user.UserId);
            var progressMap = await _achievementRepository
    .GetAchievementProgress(user.UserId, entries, attempts);

            var earnedIds = earned.Select(e => e.AchievementId).ToHashSet();

            // Filter: earned + one next tier per type
            var nextTiers = achievements
                .GroupBy(a => a.ConditionType)
                .Select(g => g.OrderBy(a => a.Threshold)
                    .FirstOrDefault(a => !earnedIds.Contains(a.AchievementId)))
                .Where(a => a != null)
                .ToList();

            var displayAchievements = achievements
            .GroupBy(a => a.ConditionType)
            .Select(group =>
            {
                var earnedForType = group
                    .Where(a => earnedIds.Contains(a.AchievementId))
                    .OrderByDescending(a => a.Threshold)
                    .FirstOrDefault();

                if (earnedForType != null)
                {
                    var nextTier = group
                        .Where(a => a.Threshold > earnedForType.Threshold)
                        .OrderBy(a => a.Threshold)
                        .FirstOrDefault();

                    return nextTier ?? earnedForType;
                }

                // If none earned, return first tier
                return group.OrderBy(a => a.Threshold).First();
            })
            .Where(a => a != null)
            .ToList();

            // Map achievements into AchievementDto list
           var achievementsDto = displayAchievements.Select(a =>
            {
                var tier = achievements
                    .Where(x => x.ConditionType == a.ConditionType)
                    .OrderBy(x => x.Threshold)
                    .Select((x, index) => new { x.AchievementId, Tier = index + 1 })
                    .FirstOrDefault(x => x.AchievementId == a.AchievementId)?.Tier ?? 1;

                return new AchievementDto
                {
                    AchievementId = a.AchievementId,
                    Name = a.Name,
                    Description = a.Description,
                    Total = a.Threshold,
                    Progress = progressMap.GetValueOrDefault(a.AchievementId, 0),
                    EarnedAt = earned.FirstOrDefault(e => e.AchievementId == a.AchievementId)?.EarnedAt,
                    Tier = tier
                };
            }).ToList();

            var userModel = new PublicUserModel
            {
                Name = user.Name ?? "",
                NickName = user.NickName ?? "",
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

        [HttpGet("getUserEndAddress")]
        public IActionResult GetMyEndAddress()
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
            var address = _endUserAddressRepository.GetUserEndAddress(user.UserId);
            if (address == null)
                return NotFound("No end address found for user.");

            return Ok(address);
        }

        [HttpPut("companySet")]
        public IActionResult SetUserCompany([FromBody] UpdateProfileOnboarding userModel)
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
