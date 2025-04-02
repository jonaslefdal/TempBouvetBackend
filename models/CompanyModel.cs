using System.ComponentModel.DataAnnotations;

namespace BouvetBackend.Models.CompleteChallengeModel
{
    public class CompanyModel
    {
        public int CompanyId { get; set; }
        public required string Name { get; set; }

    }
}
