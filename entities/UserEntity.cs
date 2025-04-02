using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BouvetBackend.Entities
{
    [Table("users")]
    public class Users
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public required string AzureId { get; set; }
        [Required]
        public required string Email { get; set; }
        public string? Name { get; set; }

        [ForeignKey("Company")]
        public int? CompanyId { get; set; }
        public virtual Company? Company { get; set; }
        // Cached total score for performance
        public int TotalScore { get; set; } = 0;
        public string? NickName { get; set; }
        public string? Address { get; set; }

        [ForeignKey("Team")]
        public int? TeamId { get; set; }
        public string? ProfilePicture { get; set; }
        public virtual Teams? Team { get; set; }
        public virtual ICollection<TransportEntry> TransportEntry { get; set; } = new List<TransportEntry>();
        public virtual ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();

    }
}
