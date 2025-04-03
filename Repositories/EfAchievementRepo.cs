using BouvetBackend.Entities;
using BouvetBackend.DataAccess;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using BouvetBackend.Extensions;
using BouvetBackend.DTO;

namespace BouvetBackend.Repositories
{

        /// <summary>
        /// Lengde gåing, sykling, bussing og bul
        /// totalt reister totalt for 4 Methode 
        /// antall fullført custom challenges 
        /// Nå en viss mengde poeng 
        /// hvor mye co2 bespart
        /// hvor mye penger bespart 
        /// antall unclucked challenges 
        /// </summary>


    public class EfAchievementRepository : IAchievementRepository
    {
        private readonly DataContext _context;

        public EfAchievementRepository(DataContext context)
        {
            _context = context;
        }

        private async Task<int> WeeklyCount(int userId, Methode method)
        {
            var weekStart = DateTime.UtcNow.StartOfWeek(DayOfWeek.Monday);
            return await _context.TransportEntry
                .CountAsync(te => te.UserId == userId && te.Method == method && te.CreatedAt >= weekStart);
        }

        private async Task<int> CountForMethod(int userId, Methode method)
        {
            return await _context.TransportEntry
                .CountAsync(te => te.UserId == userId && te.Method == method);
        }

        private async Task<int> WeeklyTransportMethods(int userId)
        {
            var weekStart = DateTime.UtcNow.StartOfWeek(DayOfWeek.Monday);
            return await _context.TransportEntry
                .Where(te => te.UserId == userId && te.CreatedAt >= weekStart)
                .Select(te => te.Method)
                .Distinct()
                .CountAsync();
        }

        private bool IsMidnightRide(TransportEntry? entry)
        {
            if (entry == null) return false;
            var norwayTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Oslo");
            var entryLocalTime = TimeZoneInfo.ConvertTimeFromUtc(entry.CreatedAt, norwayTimeZone);
            return entryLocalTime.Hour >= 0 && entryLocalTime.Hour < 5;
        }

        public void Upsert(Achievement achievement)
        {
            var existing = _context.Achievement.FirstOrDefault(a => a.AchievementId == achievement.AchievementId);
            if (existing != null)
            {
                existing.Name = achievement.Name;
                existing.ConditionType = achievement.ConditionType;
                existing.Threshold = achievement.Threshold;
            }
            else
            {
                _context.Achievement.Add(achievement);
            }

            _context.SaveChanges();
        }

        public List<UserAchievement> GetUserAchievements(int userId)
        {
            return _context.UserAchievement
                .Where(ua => ua.UserId == userId)
                .ToList();
        }

        public Achievement? Get(int achievementId)
        {
            return _context.Achievement.FirstOrDefault(a => a.AchievementId == achievementId);
        }

        public List<Achievement> GetAll()
        {
            return _context.Achievement.ToList();
        }

        public List<Achievement> GetByCondition(AchievementCondition conditionType)
        {
            return _context.Achievement.Where(a => a.ConditionType == conditionType).ToList();
        }

        private async Task<UserStats> GetUserStats(
            int userId,
            List<TransportEntry> entries,
            List<UserChallengeProgress> userChallenges)
        {
            var challengeIds = userChallenges.Select(p => p.ChallengeId).Distinct().ToList();

            // Only fetch the relevant challenges once
            var challenges = await _context.Challenge
                .Where(c => challengeIds.Contains(c.ChallengeId))
                .ToListAsync();

            var totalDistanceByMethod = entries
                .GroupBy(e => e.Method)
                .ToDictionary(g => g.Key, g => g.Sum(e => e.DistanceKm));

            return new UserStats
            {
                TotalEntries = entries.Count,
                TotalDistance = entries.Sum(e => e.DistanceKm),
                BusDistance = entries.Where(e => e.Method == Methode.Bus).Sum(e => e.DistanceKm),
                TotalPoints = entries.Sum(e => e.Points),
                TotalCo2 = entries.Sum(e => e.Co2),
                TotalMoney = entries.Sum(e => e.MoneySaved),
                CustomChallengeCount = challenges.Count(c => c.RequiredTransportMethod == Methode.Custom),
                EarnedAchievementCount = await _context.UserAchievement.CountAsync(ua => ua.UserId == userId),
                UnlockedChallengeCount = challengeIds.Count(),
                EcoFriendlyCount = entries.Count(e => e.Method == Methode.Cycling || e.Method == Methode.Bus),
                LastEntryMethod = entries.LastOrDefault()?.Method,
                LastEntryTimeUtc = entries.LastOrDefault()?.CreatedAt,
                TotalDistanceByMethod = totalDistanceByMethod
            };
        }


        public async Task CheckForAchievements(
            int userId,
            Methode method,
            List<TransportEntry> entries,
            List<UserChallengeProgress> userChallenges,
            TransportEntry? entry = null)
        {
            var allAchievements = await _context.Achievement.ToListAsync();
            var earned = await _context.UserAchievement
                .Where(ua => ua.UserId == userId)
                .ToListAsync();
            var earnedIds = earned.Select(e => e.AchievementId).ToHashSet();

            var stats = await GetUserStats(userId, entries, userChallenges);

            // Group by condition and find the lowest unearned tier per type
            var nextTiers = allAchievements
                .GroupBy(a => a.ConditionType)
                .Select(g => g.OrderBy(a => a.Threshold)
                    .FirstOrDefault(a => !earnedIds.Contains(a.AchievementId)))
                .Where(a => a != null)
                .ToList();

            foreach (var achievement in nextTiers)
            {
                if (achievement is null) continue;

                if (await IsAchievementMet(achievement.ConditionType, achievement.Threshold, stats, userId, method, entry))
                {
                    await AwardAchievement(userId, achievement);
                }
            }
        }


        private Task<bool> IsAchievementMet(
            AchievementCondition condition,
            int threshold,
            UserStats stats,
            int userId,
            Methode method,
            TransportEntry? entry)
        {
            var result = condition switch
            {
                AchievementCondition.DistanceWalking => stats.TotalDistanceByMethod.TryGetValue(Methode.Walking, out var walk) && walk >= threshold,
                AchievementCondition.DistanceCycling => stats.TotalDistanceByMethod.TryGetValue(Methode.Cycling, out var cycle) && cycle >= threshold,
                AchievementCondition.DistanceBus => stats.TotalDistanceByMethod.TryGetValue(Methode.Bus, out var bus) && bus >= threshold,
                AchievementCondition.DistanceCar => stats.TotalDistanceByMethod.TryGetValue(Methode.Car, out var car) && car >= threshold,
                AchievementCondition.TotalEntries => stats.TotalEntries >= threshold,
                AchievementCondition.CustomChallengeCount => stats.CustomChallengeCount >= threshold,
                AchievementCondition.PointsTotal => stats.TotalPoints >= threshold,
                AchievementCondition.Co2SavedTotal => stats.TotalCo2 >= threshold,
                AchievementCondition.MoneySavedTotal => stats.TotalMoney >= threshold,
                AchievementCondition.UnlockedChallengeCount => stats.UnlockedChallengeCount >= threshold,
                AchievementCondition.AchievementsUnlockedCount => stats.EarnedAchievementCount >= threshold,
                _ => false
            };

            return Task.FromResult(result);
        }

        private async Task AwardAchievement(int userId, Achievement achievement)
        {
            var userAchievement = new UserAchievement
            {
                UserId = userId,
                AchievementId = achievement.AchievementId,
                EarnedAt = DateTime.UtcNow
            };

            _context.UserAchievement.Add(userAchievement);
            await _context.SaveChangesAsync();            
        }


        public async Task<Dictionary<int, int>> GetAchievementProgress(
        int userId,
        List<TransportEntry> entries,
        List<UserChallengeProgress> userChallenges)
    {
        var progress = new Dictionary<int, int>();

        var stats = await GetUserStats(userId, entries, userChallenges);
        var allAchievements = _context.Achievement.ToList();

        foreach (var a in allAchievements)
        {
            switch (a.ConditionType)
            {
            case AchievementCondition.DistanceWalking:
                progress[a.AchievementId] = (int)Math.Floor(
                    entries.Where(te => te.Method == Methode.Walking).Sum(te => te.DistanceKm));
                break;

            case AchievementCondition.DistanceCycling:
                progress[a.AchievementId] = (int)Math.Floor(
                    entries.Where(te => te.Method == Methode.Cycling).Sum(te => te.DistanceKm));
                break;

            case AchievementCondition.DistanceBus:
                progress[a.AchievementId] = (int)Math.Floor(
                    entries.Where(te => te.Method == Methode.Bus).Sum(te => te.DistanceKm));
                break;

            case AchievementCondition.DistanceCar:
                progress[a.AchievementId] = (int)Math.Floor(
                    entries.Where(te => te.Method == Methode.Car).Sum(te => te.DistanceKm));
                break;

            case AchievementCondition.TotalEntries:
                progress[a.AchievementId] = entries.Count;
                break;

            case AchievementCondition.CustomChallengeCount:
                progress[a.AchievementId] = stats.CustomChallengeCount;
                break;

            case AchievementCondition.PointsTotal:
                progress[a.AchievementId] = stats.TotalPoints;
                break;

            case AchievementCondition.Co2SavedTotal:
                progress[a.AchievementId] = (int)Math.Floor(stats.TotalCo2);
                break;

            case AchievementCondition.MoneySavedTotal:
                progress[a.AchievementId] = (int)Math.Floor(stats.TotalMoney);
                break;

            case AchievementCondition.UnlockedChallengeCount:
                progress[a.AchievementId] = stats.UnlockedChallengeCount;
                break;

            case AchievementCondition.AchievementsUnlockedCount:
                progress[a.AchievementId] = stats.EarnedAchievementCount;
                break;

            default:
                progress[a.AchievementId] = 0;
                break;
            }
        }

        return progress;
    }



    }
}
