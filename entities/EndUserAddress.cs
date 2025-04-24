using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BouvetBackend.Entities
{
    [Table("enduseraddress")]
    public class EndUserAddress
    {
        [Key]
        public int EndUserAddressId { get; set; }
        public required string EndAddress { get; set; } 
        [ForeignKey("Users")]
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}