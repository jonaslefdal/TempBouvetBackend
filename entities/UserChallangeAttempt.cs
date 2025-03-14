using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BouvetBackend.Entities
{
    [Table("userChallengeAttempts")]
    public class UserChallengeAttempt
    {
        [Key]
        public int UserChallengeAttemptId { get; set; }

        [ForeignKey("Users")]
        public int UserId { get; set; }
        public virtual Users? Users { get; set; }

        [ForeignKey("Challenge")]
        public int ChallengeId { get; set; }
        public virtual Challenge? Challenge { get; set; }

        // Kanskje ha poeng lagret
        public int PointsAwarded { get; set; }

        public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;
    }
}
