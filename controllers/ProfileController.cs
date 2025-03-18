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

        public ProfileController(ICompanyRepository companyRepository,IUserRepository userRepository, ITransportEntryRepository transportEntryRepository)
        {
            _companyRepository = companyRepository;
            _userRepository = userRepository;
            _transportEntryRepository = transportEntryRepository;
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
    }
}
