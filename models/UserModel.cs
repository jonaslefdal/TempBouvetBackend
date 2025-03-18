using System;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace BouvetBackend.Models.UserModel
{

    public class CompositeViewModel
    {
        public UserModel ?UserModel { get; set; }
            }
    public class UserFullModel
    {
        public UserFullModel()
        {
            UpsertModel = new UserModel();
            UserModelList = new List<UserModel>();
        }

        public UserModel UpsertModel { get; set; }
        public List<UserModel> UserModelList { get; set; }
    }

     public class UserModel
    {
        public int UserId { get; set; } 
        public string? AzureId { get; set; } 
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? NickName { get; set; }
        public string? Address { get; set; }
        public int CompanyId { get; set; }
        public int TotalScore { get; set; }
        public int? TeamId { get; set; }
    }

}
