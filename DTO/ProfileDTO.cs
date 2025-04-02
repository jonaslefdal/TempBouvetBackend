using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BouvetBackend.Models.UserModel;

namespace BouvetBackend.DTO
{

public class ProfileOverviewDto
{
    public required PublicUserModel User { get; set; } 
    public double TotalCo2Savings { get; set; }
    public int TotalTravels { get; set; }
    public double TotalMoneySaved { get; set; }
    public int CompletedChallenges { get; set; }
    public required IEnumerable<AchievementDto> Achievements { get; set; }
}

public class AchievementDto
{
    public int AchievementId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public int Total { get; set; }
    public int Progress { get; set; }
    public DateTime? EarnedAt { get; set; }
    public int Tier { get; set; } 

}
}