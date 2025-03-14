using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BouvetBackend.Entities
{
    [Table("teams")]
    public class Teams
    {
        [Key]
        public int TeamId { get; set; }
        [Required]
        public string Name { get; set; }
        // Foreign key to Company: Each team belongs to one company.
        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }
        // Navigation property for users in this team.
        public virtual ICollection<Users> Users { get; set; } = new List<Users>();
    }
}
