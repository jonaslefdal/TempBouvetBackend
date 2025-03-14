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

        public ProfileController(ICompanyRepository companyRepository,IUserRepository userRepository)
        {
            _companyRepository = companyRepository;
            _userRepository = userRepository;
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
            if (userModel == null || string.IsNullOrEmpty(userModel.Email))
            {
                return BadRequest("Invalid request data.");
            }
            var user = _userRepository.GetUserByEmail(userModel.Email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Update the user's company
            user.CompanyId = userModel.CompanyId;
            _userRepository.InsertOrUpdateUser(user);

            return Ok(new { message = "User company updated successfully." });
        }


    }
}
