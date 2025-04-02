using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BouvetBackend.Entities;
using BouvetBackend.Models.UserModel;

namespace BouvetBackend.DTO
{

public class UserStats
{
    public int TotalEntries { get; set; }
    public double TotalDistance { get; set; }
    public double BusDistance { get; set; }
    public int TotalPoints { get; set; }
    public double TotalCo2 { get; set; }
    public double TotalMoney { get; set; }
    public int CustomChallengeCount { get; set; }
    public int EarnedAchievementCount { get; set; }
    public int UnlockedChallengeCount { get; set; }
    public int EcoFriendlyCount { get; set; }
    public Methode? LastEntryMethod { get; set; }
    public DateTime? LastEntryTimeUtc { get; set; }
    public Dictionary<Methode, double> TotalDistanceByMethod { get; set; } = new();
}
}