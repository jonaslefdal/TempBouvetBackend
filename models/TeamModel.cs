using System;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace BouvetBackend.Models.TeamModel
{

    public class CompositeViewModel
    {
        public TeamModel ?UserModel { get; set; }
            }
    public class TeamFullModel
    {
        public TeamFullModel()
        {
            UpsertModel = new TeamModel();
            TeamModelList = new List<TeamModel>();
        }

        public TeamModel UpsertModel { get; set; }
        public List<TeamModel> TeamModelList { get; set; }
    }

     public class TeamModel
    {
        [Required]
        public int TeamId { get; set; }   
        [Required]
        public string? Name { get; set; }
        [Required]        
        public int CompanyId { get; set; }

    }
    public class JoinTeamModel
    {
        public string? Email { get; set; }
        public int TeamId { get; set; }
    }

}
