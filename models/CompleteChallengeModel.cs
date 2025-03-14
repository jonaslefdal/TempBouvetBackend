using System.ComponentModel.DataAnnotations;

namespace BouvetBackend.Models.CompleteChallengeModel
{
    public class CompleteChallengeRequest
    {
        [Required]
        public string? Email { get; set; }
        
        [Required]
        public int ChallengeId { get; set; }
    }
}
