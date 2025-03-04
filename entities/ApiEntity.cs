using System.ComponentModel.DataAnnotations;//Importerer for å validere modeller
using System.ComponentModel.DataAnnotations.Schema;//Importerer for å tilkoble modellene til databasetabeller.
using BouvetBackend.Models.ApiModel;

namespace BouvetBackend.Entities//Definerer navneområde for entitetsklasser
{
    // Definerer Customer-klasse som tilsvarer customer-tabell i database.
    [Table("api")]
    public class API
    {
        [Key]
        public int apiId { get; set; }
        public int value1 { get; set; }

        public int value2 { get; set; }


    }
}