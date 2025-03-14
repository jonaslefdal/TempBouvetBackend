using System;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;
namespace BouvetBackend.Models.ChallengeModel
{

    public class CompositeViewModel
    {
        public ChallengeModel ?ChallengeModel { get; set; }
            }
    public class ChallengeFullModel
    {
        public ChallengeFullModel()
        {
            UpsertModel = new ChallengeModel();
            ChallengeModelList = new List<ChallengeModel>();
        }

        public ChallengeModel UpsertModel { get; set; }
        public List<ChallengeModel> ChallengeModelList { get; set; }
    }

    public class ChallengeModel
    {
        public int UserId { get; set; }
        [Required]
        public string ?Email { get; set; } 
        public string ?Method { get; set; }  
        public int Points { get; set; }     
        public int RotationGroup { get; set; }     
    }
}



