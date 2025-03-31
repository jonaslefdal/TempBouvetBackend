using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BouvetBackend.Models.UserModel;

namespace BouvetBackend.DTO
{

public class ProfileOverviewDto
{
    public UserModel User { get; set; } 
    public double TotalCo2Savings { get; set; }
    public int TotalTravels { get; set; }
    public double TotalMoneySaved { get; set; }
    public int CompletedChallenges { get; set; }
    public IEnumerable<AchievementDto> Achievements { get; set; }
}

public class AchievementDto
{
    public int AchievementId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Total { get; set; }
    public int Progress { get; set; }
    public DateTime? EarnedAt { get; set; }
}
}