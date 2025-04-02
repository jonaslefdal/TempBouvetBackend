using System;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace BouvetBackend.Models.TeamModel
{
     public class TeamModel
    {
        [Required]
        public int TeamId { get; set; }   
        [Required]
        public required string Name { get; set; }
        [Required] 
        public int CompanyId { get; set; }
        public int MaxMembers { get; set; } = 5;
        public int MemberCount { get; set; }

    }
    public class JoinTeamModel
    {
        public int TeamId { get; set; }
    }
}
