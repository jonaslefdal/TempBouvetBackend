using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BouvetBackend.Entities
{
    [Table("transportEntries")]
    public class TransportEntry
    {
        [Key]
        public int TransportEntryId { get; set; }
        [ForeignKey("Users")]
        public int UserId { get; set; }
        public virtual Users ?Users { get; set; }
        [Required]
        public string ?Method { get; set; }
        public double Co2 { get; set; }
        public double DistanceKm { get; set; }
        // Points awarded for this submission
        public int Points { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
