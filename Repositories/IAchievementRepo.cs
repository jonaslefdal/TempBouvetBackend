using BouvetBackend.Entities;
using System.Collections.Generic;

namespace BouvetBackend.Repositories
{
    public interface IAchievementRepository
    {
        void Upsert(Achievement achievement);
        Achievement? Get(int achievementId);
        List<Achievement> GetAll();
        List<Achievement> GetByCondition(string conditionType);
        Task CheckForAchievements(int userId, string activityType);
        List<UserAchievement> GetUserAchievements(int id);

    }
}
