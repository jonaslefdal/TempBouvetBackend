using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BouvetBackend.Entities
{
    [Table("companies")]
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }

        [Required]
        public string ?Name { get; set; }

        // Navigation property: One company has many users.
        public virtual ICollection<Users> Users { get; set; } = new List<Users>();
    }
}
