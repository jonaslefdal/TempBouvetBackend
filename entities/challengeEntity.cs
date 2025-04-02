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
        [Required]
        public int RotationGroup { get; set; }
        public int? MaxAttempts { get; set; }
        public Methode RequiredTransportMethod { get; set; }
        public string? ConditionType { get; set; } // "Standard", "Custom", "Distance"
        public double? RequiredDistanceKm { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}


