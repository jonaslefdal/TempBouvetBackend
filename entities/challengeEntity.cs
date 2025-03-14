using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BouvetBackend.Entities
{
    [Table("challenges")]
    public class Challenge
    {
        [Key]
        public int ChallengeId { get; set; }

        [Required]
        public string ?Description { get; set; }

        // Points awarded for completing this challenge
        [Required]
        public int Points { get; set; }

        // Maximum number of times a user can complete this challenge
        [Required]
        public int MaxAttempts { get; set; }
        [Required]
        public int RotationGroup { get; set; }

        // Date and time when the challenge was created
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
