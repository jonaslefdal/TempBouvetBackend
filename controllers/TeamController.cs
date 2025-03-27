using Microsoft.AspNetCore.Mvc;
using BouvetBackend.Repositories;
using BouvetBackend.Entities;
using BouvetBackend.Models.TeamModel;
using Microsoft.AspNetCore.Authorization;

namespace BouvetBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TeamController : Controller
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;

        public TeamController(ITeamRepository teamRepository, IUserRepository userRepository)
        {
            _teamRepository = teamRepository;
            _userRepository = userRepository;
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
            var teams = _teamRepository.GetTeamsByCompanyId(user.CompanyId ?? 0);
            
            if (teams == null || teams.Count == 0)
                return NotFound("No teams found for your company.");

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
            Console.WriteLine(team);

            // Optionally, check that the team.CompanyId matches the user's company.
            var email = User.FindFirst("emails")?.Value;
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email claim missing.");

                Console.WriteLine(email);

            var user = _userRepository.GetUserByEmail(email);
            if (user == null)
                return NotFound("User not found.");

                Console.WriteLine(user);

            if (user.CompanyId != team.CompanyId)
            {
                Console.WriteLine("user.CompanyId != team.CompanyId)");
                return BadRequest("Cannot create or update teams outside your company.");
            }

                var entity = new Teams
                {
                    Name = team.Name,
                    CompanyId = team.CompanyId,
                };

            _teamRepository.Upsert(entity);
            return Ok(new { message = "Team upserted successfully." });
        }

        [HttpPut("join")]
        public IActionResult JoinTeam([FromBody] JoinTeamModel model)
        {
            if (model == null)
                return BadRequest("Invalid data.");

            var email = User.FindFirst("preferred_username")?.Value 
                        ?? User.FindFirst("emails")?.Value;
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
                    MemberCount = team.Users != null ? team.Users.Count() : 0, 
                    TeamTotalScore = team.Users.Sum(user => user.TotalScore),
                }).ToList();

                return Ok(leaderboard);
            }
    }
}
