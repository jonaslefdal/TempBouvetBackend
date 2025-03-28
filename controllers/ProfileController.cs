using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BouvetBackend.Models.UserModel;
using BouvetBackend.Entities;
using BouvetBackend.Repositories;

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

        public ProfileController(ICompanyRepository companyRepository, 
        IUserRepository userRepository, 
        ITransportEntryRepository transportEntryRepository,
        IChallengeRepository challengeRepository,
        IUserChallengeProgressRepository challengeProgressRepository
        )
        {
            _companyRepository = companyRepository;
            _userRepository = userRepository;
            _transportEntryRepository = transportEntryRepository;
            _challengeRepository = challengeRepository;
            _challengeProgressRepository = challengeProgressRepository;
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

            // Group attempts by challenge
            var attemptsGrouped = attempts
                .GroupBy(a => a.ChallengeId)
                .Select(g => new { ChallengeId = g.Key, AttemptCount = g.Count() })
                .ToList();

            // Get all challenges (so we can see each challenge's MaxAttempts)
            var allChallenges = _challengeRepository.GetAll();
            var challengeLookup = allChallenges.ToDictionary(c => c.ChallengeId, c => c);

            // Count how many are completed
            int completedCount = 0;
            foreach (var group in attemptsGrouped)
            {
                if (challengeLookup.TryGetValue(group.ChallengeId, out var challenge))
                {
                    // If user attempts >= challenge.MaxAttempts => fully completed
                    if (group.AttemptCount >= challenge.MaxAttempts)
                    {
                        completedCount++;
                    }
                }
            }

            // Return just the count, or an object with more details if you like
            return Ok(new { completedChallenges = completedCount });
        }

    }
}
