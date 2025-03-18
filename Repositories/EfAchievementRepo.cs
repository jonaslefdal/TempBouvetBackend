using BouvetBackend.Entities;
using BouvetBackend.DataAccess;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

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

                if (walkingAchievement != null && !userAchievements.Contains(walkingAchievement.AchievementId))
                {
                    await AwardAchievement(userId, walkingAchievement);
                }
            }
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
    }
}
