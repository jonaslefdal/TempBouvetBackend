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
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("upsert")]
        public IActionResult UpsertUser([FromBody] UserModel userModel)
        {
            if (userModel == null)
            {
                return BadRequest("Invalid user data.");
            }

            var azureId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst("emails")?.Value; 
            var givenName = User.FindFirst(ClaimTypes.GivenName)?.Value;    
            var lastName = User.FindFirst(ClaimTypes.Surname)?.Value;    

            var name = $"{givenName} {lastName}".Trim();

            if (string.IsNullOrEmpty(azureId) || string.IsNullOrEmpty(email))
            {
                return Unauthorized("Invalid token data.");
            }

            var entity = new Users
            {
                AzureId = azureId,
                Email = email,
                Name = name,
                TotalScore = 0
            };

            _userRepository.InsertOrUpdateUser(entity);

            return Ok(new { message = "User upserted successfully." });
        }


        [HttpGet("all")]
        public IActionResult GetAllUsers()
        {
            var users = _userRepository.GetAllUsers();
            if (users == null || users.Count == 0)
            {
                return NotFound("No users found.");
            }
            
            var leaderboard = users.Select(user => new
            {
                user.UserId,
                user.Name,
                user.TotalScore
            }).ToList(); 

            return Ok(leaderboard);
        }

    }
}
