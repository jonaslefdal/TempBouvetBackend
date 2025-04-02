using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BouvetBackend.Entities
{
public enum AchievementCondition
{
    DistanceWalking,
    DistanceCycling,
    DistanceBus,
    DistanceCar,

    TotalEntries,

    CustomChallengeCount,
    PointsTotal,
    Co2SavedTotal,
    MoneySavedTotal,
    UnlockedChallengeCount
}

    [Table("achievements")]
    public class Achievement
    {
        [Key]
        public int AchievementId { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public required AchievementCondition ConditionType { get; set; } 
        public int Threshold { get; set; }
        public required string Description { get; set; }
    }
}
