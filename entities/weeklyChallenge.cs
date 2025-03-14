using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BouvetBackend.Entities
{
    [Table("weeklychallenges")]
    public class WeeklyChallenge
    {
        [Key]
        public int WeeklyChallengeId { get; set; }

        // The start date for the week these challenges are active.
        [Required]
        public DateTime WeekStartDate { get; set; }

        // Foreign key to the master Challenge.
        [ForeignKey("Challenge")]
        public int ChallengeId { get; set; }
        public virtual Challenge? Challenge { get; set; }
        public int DisplayOrder { get; set; }
    }
}
