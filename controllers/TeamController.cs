using Microsoft.AspNetCore.Mvc;
using BouvetBackend.Repositories;
using BouvetBackend.Entities;
using BouvetBackend.Models.TeamModel;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;

namespace BouvetBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TeamController : Controller
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICompanyRepository _companyRepository;

        public TeamController(ITeamRepository teamRepository, IUserRepository userRepository, ICompanyRepository companyRepository)
        {
            _teamRepository = teamRepository;
            _userRepository = userRepository;
            _companyRepository = companyRepository;
        }

        // GET: api/team/company
        [HttpGet("company")]
        public IActionResult GetTeamsForMyCompany()
        {
            // Get user email from token (adjust claim extraction as needed)
            var email = User.FindFirst("emails")?.Value;
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email claim missing.");

            var user = _userRepository.GetUserByEmail(email);
            if (user == null)
                return NotFound("User not found.");

            // Return teams for the user’s company.
            if (user.CompanyId == null)
            {
                return StatusCode(428, new { message = "User has not completed onboarding." });
            }

            var teams = _teamRepository.GetTeamsByCompanyId(user.CompanyId.Value);

            
            if (teams == null || teams.Count == 0)
                return Ok(new List<object>()); // Return empty list instead of 404

                var result = teams.Select(team => new {
                TeamId = team.TeamId,
                Name = team.Name,
                MemberCount = team.MemberCount, 
                MaxMembers = team.MaxMembers
            });

            return Ok(result);
        }

        // POST: api/team/upsert
        [HttpPost("upsert")]
        public IActionResult UpsertTeam([FromBody] TeamModel team)
        {
            if (team == null)
                return BadRequest("Invalid team data.");

            // Optionally, check that the team.CompanyId matches the user's company.
            var email = User.FindFirst("emails")?.Value;
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email claim missing.");

            var user = _userRepository.GetUserByEmail(email);
            if (user == null)
                return NotFound("User not found.");

            if (user.CompanyId == null)
            {
                return StatusCode(428, new { message = "User has not completed onboarding." });
            }

                team.CompanyId = user.CompanyId.Value;

                var entity = new Teams
                {
                    Name = team.Name,
                    CompanyId = team.CompanyId,
                };

            _teamRepository.Upsert(entity);

            user.TeamId = entity.TeamId;
            _userRepository.InsertOrUpdateUser(user);

            return Ok(new { message = "Team upserted successfully." });
        }

        [HttpPut("join")]
        public IActionResult JoinTeam([FromBody] JoinTeamModel model)
        {
            if (model == null)
                return BadRequest("Invalid data.");

            var email = User.FindFirst("emails")?.Value;
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email claim missing.");

            var user = _userRepository.GetUserByEmail(email);
            if (user == null)
                return NotFound("User not found.");

            // Retrieve the team by ID
            var team = _teamRepository.Get(model.TeamId);
            if (team == null)
                return NotFound("Team not found.");

            // Ensure the team belongs to the same company as the user.
            if (team.CompanyId != user.CompanyId)
                return BadRequest("Cannot join a team from a different company.");

            if (team.Users.Count >= team.MaxMembers)
                return BadRequest("Team is full.");

            // Update the user's team
            user.TeamId = model.TeamId;
            _userRepository.InsertOrUpdateUser(user);  

            return Ok(new { message = "Joined team successfully." });
        }

        [HttpGet("myteam")]
        public IActionResult GetMyTeam()
        {
            var email = User.FindFirst("emails")?.Value;
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email claim missing.");

            var user = _userRepository.GetUserByEmail(email);
            if (user == null)
                return NotFound("User not found.");

            if (user.TeamId == null)
                return NotFound("You are not in a team.");

            var team = _teamRepository.GetTeamWithMembers((int)user.TeamId);
            if (team == null)
                return NotFound("Team not found.");

            var teamModel = new
            {
                TeamId = team.TeamId,
                Name = team.Name,
                TeamProfilePicture = team.TeamProfilePicture,
                TeamTotalScore = team.Users.Sum(u => u.TotalScore),
                Members = team.Users.Select(u => new { u.UserId, u.NickName, u.TotalScore, u.ProfilePicture }).ToList()
            };

            return Ok(teamModel);
        }

        [HttpPut("leave")]
        public IActionResult LeaveTeam()
        {
            var email = User.FindFirst("emails")?.Value;
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email claim missing.");

            var user = _userRepository.GetUserByEmail(email);
            if (user == null)
                return NotFound("User not found.");

            if (user.TeamId == null)
                return BadRequest("You are not in a team.");

            user.TeamId = null;
            _userRepository.InsertOrUpdateUser(user);

            return Ok(new { message = "You have left the team." });
        }

        [HttpPut("editTeam")]
        public IActionResult EditTeam([FromBody] EditTeamModel team)
        {
            if (team == null)
                return BadRequest("Invalid team data.");

            var email = User.FindFirst("emails")?.Value;
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email claim missing.");

            var user = _userRepository.GetUserByEmail(email);
            if (user == null)
                return NotFound("User not found.");

            if (user.CompanyId == null)
            {
                return StatusCode(428, new { message = "User has not completed onboarding." });
            }
            
            if (user.TeamId != team.TeamId)
            {
                return BadRequest("User is not in the specified team.");
            }

            if (!string.IsNullOrWhiteSpace(team.Name)) team.Name = team.Name.Trim();

            
            if (!string.IsNullOrEmpty(team.TeamProfilePicture) &&
                !Regex.IsMatch(team.TeamProfilePicture, @"^teamAvatar\d+\.png$", RegexOptions.IgnoreCase))
            {
                return BadRequest("Ugyldig profilbilde.");
            }

            var entity = new EditTeamModel
            {
                TeamId = team.TeamId,
                Name = team.Name,
                TeamProfilePicture = team.TeamProfilePicture 
            };

            _teamRepository.EditTeam(entity);

            return Ok(new { message = "Team upserted successfully." });
        }

            [HttpGet("allTeams")]
            public IActionResult GetAllTeams()
            {
                var teams = _teamRepository.GetAll(); // Fetch all teams
                
                if (teams == null || teams.Count == 0)
                {
                    return NotFound("No teams found.");
                }

                var leaderboard = teams.Select(team => new
                {
                    TeamId = team.TeamId,
                    Name = team.Name,
                    TeamProfilePicture = team.TeamProfilePicture,
                    MemberCount = team.Users != null ? team.Users.Count() : 0, 
                    TeamTotalScore = team.Users != null ? team.Users.Sum(user => user.TotalScore) : 0,
                }).ToList();

                return Ok(leaderboard);
            }

            [HttpGet("companyScores")]
            public IActionResult GetCompanyScores()
            {
                var scores = _companyRepository.GetCompanyScores();
                return Ok(scores);
            }
    }
}
