using System;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;
namespace BouvetBackend.Models.ChallengeModel
{
    public class ChallengeModel
    {
        public int UserId { get; set; }
        public required string Method { get; set; }  
        public int Points { get; set; }     
        public int RotationGroup { get; set; }     
    }
}



