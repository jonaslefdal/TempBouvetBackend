using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BouvetBackend.Entities
{
    [Table("userChallengeProgress")]
    public class UserChallengeProgress
    {
        [Key]
        public int UserChallengeProgressId { get; set; }

        [ForeignKey("Users")]
        public int UserId { get; set; }
        public virtual Users? Users { get; set; }
        
        [ForeignKey("Challenge")]
        public int ChallengeId { get; set; }
        public virtual Challenge? Challenge { get; set; }
        public int PointsAwarded { get; set; }
        public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;
    }
}
