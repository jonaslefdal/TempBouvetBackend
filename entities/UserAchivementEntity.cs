using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BouvetBackend.Entities
{
    [Table("userAchievements")]
    public class UserAchievement
    {
        [Key]
        public int UserAchievementId { get; set; }
        [ForeignKey("Users")]
        public int UserId { get; set; }
        public virtual Users? Users { get; set; }

        [ForeignKey("Achievement")]
        public int AchievementId { get; set; }
        public virtual Achievement? Achievement { get; set; }

        public DateTime EarnedAt { get; set; } = DateTime.UtcNow;
    }
}
