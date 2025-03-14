//EXAMPLE refrence

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BouvetBackend.Models.ApiModel;

namespace BouvetBackend.Entities
{
    [Table("api")]
    public class API
    {
        [Key]
        public int apiId { get; set; }
        public int value1 { get; set; }
        public int value2 { get; set; }
    }
}