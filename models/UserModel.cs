using System;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace BouvetBackend.Models.UserModel
{
     public class UserModel
    {
        public int UserId { get; set; } 
        public required string AzureId { get; set; } 
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? NickName { get; set; }
        public string? Address { get; set; }
        public int? CompanyId { get; set; }
        public int TotalScore { get; set; }
        public int? TeamId { get; set; }
        public string? ProfilePicture { get; set; }
    }

    public class PublicUserModel
    {
        public int UserId { get; set; }
        public string NickName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public int TotalScore { get; set; }
        public int? TeamId { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
