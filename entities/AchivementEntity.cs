using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BouvetBackend.Entities
{
    [Table("achievements")]
    public class Achievement
    {
        [Key]
        public int AchievementId { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string ConditionType { get; set; } = string.Empty; 
        public int Threshold { get; set; }
        public string? Description { get; set; }
    }
}
