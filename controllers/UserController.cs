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
            var token = Request.Headers["Authorization"].ToString();
                Console.WriteLine($"Received token: {token}");

            var azureId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? userModel.AzureId;
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? userModel.Email;
            var name = User.FindFirst("name")?.Value ?? userModel.Name;

            if (string.IsNullOrEmpty(azureId) || string.IsNullOrEmpty(email))
            {
                return Unauthorized("Invalid token data.");
            }

            var entity = new Users
            {
                AzureId = azureId,
                Email = email,
                Name = name,
                CompanyId = userModel.CompanyId,
                TotalScore = userModel.TotalScore
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

            var leaderboard = new List<object>();
            foreach (var user in users)
            {
                leaderboard.Add(new
                {
                    user.UserId,
                    user.Name,
                    user.Email,
                    user.TotalScore 
                });
            }

            return Ok(leaderboard);
        }

    }
}
