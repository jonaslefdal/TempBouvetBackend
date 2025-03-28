using BouvetBackend.Entities;
using BouvetBackend.DataAccess;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using BouvetBackend.Extensions;

namespace BouvetBackend.Repositories
{
    public class EfAchievementRepository : IAchievementRepository
    {
        private readonly DataContext _context;

        public EfAchievementRepository(DataContext context)
        {
            _context = context;
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

        public List<Achievement> GetByCondition(string conditionType)
        {
            return _context.Achievement.Where(a => a.ConditionType == conditionType).ToList();
        }

        public async Task CheckForAchievements(int userId, string activityType)
        {
            var achievements = await _context.Achievement.ToListAsync();
            var userAchievements = await _context.UserAchievement
                .Where(ua => ua.UserId == userId)
                .Select(ua => ua.AchievementId)
                .ToListAsync();

                    // Log all achievements in your DB
    Console.WriteLine("=== Achievements in DB ===");
    foreach (var ach in achievements)
    {
        Console.WriteLine($"ID={ach.AchievementId}, Name={ach.Name}, Condition={ach.ConditionType}, Threshold={ach.Threshold}");
    }

    // Log which achievements the user already has
    Console.WriteLine($"=== User {userId} Achievements ===");
    Console.WriteLine(string.Join(", ", userAchievements));

            // Cycling achievement (if 10+ entries)
            if (activityType == "cycling")
            {
                var cyclingCount = await _context.TransportEntry
                    .Where(a => a.UserId == userId && a.Method == "cycling")
                    .CountAsync();

                var cyclingAchievement = achievements.FirstOrDefault(a => a.ConditionType == "cycling_count" && a.Threshold <= cyclingCount);

                if (cyclingAchievement != null && !userAchievements.Contains(cyclingAchievement.AchievementId))
                {
                    await AwardAchievement(userId, cyclingAchievement);
                }
            }

            // Walking achievement (if 5+ entries in one week)
            if (activityType == "walking")
            {
                var oneWeekAgo = DateTime.UtcNow.AddDays(-7);
                var walkingCount = await _context.TransportEntry
                    .Where(a => a.UserId == userId && a.Method == "walking" && a.CreatedAt >= oneWeekAgo)
                    .CountAsync();

                var walkingAchievement = achievements.FirstOrDefault(a => a.ConditionType == "walking_weekly" && a.Threshold <= walkingCount);

        if (walkingAchievement == null)
        {
            Console.WriteLine("No matching 'walking_weekly' achievement found OR threshold not reached.");
        }
                else
        {
            Console.WriteLine($"Found a 'walking_weekly' achievement: ID={walkingAchievement.AchievementId}, Threshold={walkingAchievement.Threshold}");

                if (walkingAchievement != null && !userAchievements.Contains(walkingAchievement.AchievementId))
                {
                    Console.WriteLine("Awarding user this achievement...");
                    await AwardAchievement(userId, walkingAchievement);
                }
                 {
                Console.WriteLine("User already has this achievement. Not awarding again.");
            }
        }
            }
            // Custom challenge completions
            if (activityType == "custom")
            {
                var customCount = await _context.UserChallengeProgress
                    .Where(p => p.UserId == userId)
                    .Join(_context.Challenge,
                        progress => progress.ChallengeId,
                        challenge => challenge.ChallengeId,
                        (progress, challenge) => new { progress, challenge })
                    .CountAsync(x => x.challenge.RequiredTransportMethod == "custom");

                var customAchievements = achievements
                    .Where(a => a.ConditionType == "custom_challenge_count" && a.Threshold <= customCount)
                    .Where(a => !userAchievements.Contains(a.AchievementId));

                foreach (var achievement in customAchievements)
                    await AwardAchievement(userId, achievement);
            }

            if (activityType == "walking" || activityType == "cycling")
            {
                var distance = await _context.TransportEntry
                    .Where(a => a.UserId == userId && a.Method == activityType)
                    .SumAsync(a => a.DistanceKm);

                var distanceAchievements = achievements
                    .Where(a => a.ConditionType == $"{activityType}_distance_total" && a.Threshold <= distance)
                    .Where(a => !userAchievements.Contains(a.AchievementId));

                foreach (var achievement in distanceAchievements)
                    await AwardAchievement(userId, achievement);
            }
            if (activityType is "walking" or "cycling" or "bus" or "car")
            {
                var weekStart = DateTime.UtcNow.StartOfWeek(DayOfWeek.Monday);

                var methodsUsed = await _context.TransportEntry
                    .Where(te => te.UserId == userId && te.CreatedAt >= weekStart)
                    .Select(te => te.Method)
                    .Distinct()
                    .ToListAsync();

                if (methodsUsed.Count >= 4)
                {
                    var versatile = achievements.FirstOrDefault(a => a.ConditionType == "versatile_weekly");
                    if (versatile != null && !userAchievements.Contains(versatile.AchievementId))
                    {
                        await AwardAchievement(userId, versatile);
                    }
                }
            }

            var now = DateTime.UtcNow;
            if (activityType != null && now.Hour >= 0 && now.Hour < 5)
            {
                var midnight = achievements.FirstOrDefault(a => a.ConditionType == "midnight_ride");
                if (midnight != null && !userAchievements.Contains(midnight.AchievementId))
                {
                    await AwardAchievement(userId, midnight);
                }
            }

            if (activityType == "car") {
            var carCount = await _context.TransportEntry
                .Where(a => a.UserId == userId && a.Method == "car")
                .CountAsync();
            
            var carAchievements = achievements
                .Where(a => a.ConditionType == "car_count" && a.Threshold <= carCount)
                .Where(a => !userAchievements.Contains(a.AchievementId));

            foreach (var a in carAchievements)
                await AwardAchievement(userId, a);
            }

            if (activityType == "bus") {
            var busCount = await _context.TransportEntry
                .Where(a => a.UserId == userId && a.Method == "bus")
                .CountAsync();
            
            var busAchievements = achievements
                .Where(a => a.ConditionType == "bus_count" && a.Threshold <= busCount)
                .Where(a => !userAchievements.Contains(a.AchievementId));

            foreach (var a in busAchievements)
                await AwardAchievement(userId, a);
        }

        var totalCount = await _context.TransportEntry
            .Where(a => a.UserId == userId)
            .CountAsync();

        var totalAchievements = achievements
            .Where(a => a.ConditionType == "total_entries" && a.Threshold <= totalCount)
            .Where(a => !userAchievements.Contains(a.AchievementId));

        foreach (var a in totalAchievements)
            await AwardAchievement(userId, a);


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
        public Dictionary<int, int> GetAchievementProgress(int userId)
        {
            var progress = new Dictionary<int, int>();

            var now = DateTime.UtcNow;
            var weekStart = now.StartOfWeek(DayOfWeek.Monday);
            var oneWeekAgo = now.AddDays(-7);

            // Total entries
            var totalEntries = _context.TransportEntry.Count(te => te.UserId == userId);

            // Distance per method
            var distances = _context.TransportEntry
                .Where(te => te.UserId == userId)
                .GroupBy(te => te.Method)
                .Select(g => new { Method = g.Key, Distance = g.Sum(te => te.DistanceKm) })
                .ToDictionary(x => x.Method, x => x.Distance);

            // Count per method
            var counts = _context.TransportEntry
                .Where(te => te.UserId == userId)
                .GroupBy(te => te.Method)
                .Select(g => new { Method = g.Key, Count = g.Count() })
                .ToDictionary(x => x.Method, x => x.Count);

            // Walking last 7 days
            var walkingWeekly = _context.TransportEntry
                .Count(te => te.UserId == userId && te.Method == "walking" && te.CreatedAt >= oneWeekAgo);

            // Custom challenge completions
            var customCompleted = _context.UserChallengeProgress
                .Where(p => p.UserId == userId)
                .Join(_context.Challenge,
                    p => p.ChallengeId,
                    c => c.ChallengeId,
                    (p, c) => c)
                .Count(c => c.RequiredTransportMethod == "custom");

            // Methods used this week
            var methodsUsedThisWeek = _context.TransportEntry
                .Where(te => te.UserId == userId && te.CreatedAt >= weekStart)
                .Select(te => te.Method)
                .Distinct()
                .Count();

            // Map progress by condition type
            foreach (var a in _context.Achievement)
            {
                switch (a.ConditionType)
                {
                    case "total_entries":
                        progress[a.AchievementId] = totalEntries;
                        break;

                    case "walking_weekly":
                        progress[a.AchievementId] = walkingWeekly;
                        break;

                    case "custom_challenge_count":
                        progress[a.AchievementId] = customCompleted;
                        break;

                    case "versatile_weekly":
                        progress[a.AchievementId] = methodsUsedThisWeek;
                        break;

                    case "cycling_count":
                    case "walking_count":
                    case "bus_count":
                    case "car_count":
                        var type = a.ConditionType.Replace("_count", "");
                        progress[a.AchievementId] = counts.GetValueOrDefault(type, 0);
                        break;

                    case "cycling_distance_total":
                    case "walking_distance_total":
                        var dType = a.ConditionType.Replace("_distance_total", "");
                        progress[a.AchievementId] = (int)Math.Floor(distances.GetValueOrDefault(dType, 0));
                        break;

                    case "midnight_ride":
                        progress[a.AchievementId] = 0; // cannot track this after-the-fact
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
