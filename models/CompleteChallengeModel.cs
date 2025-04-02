using System.ComponentModel.DataAnnotations;

namespace BouvetBackend.Models.CompleteChallengeModel
{
    public class CompleteChallengeRequest
    {
        [Required]
        public int ChallengeId { get; set; }

    }
}
