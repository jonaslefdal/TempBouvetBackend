using BouvetBackend.Entities;
using System.Collections.Generic;

namespace BouvetBackend.Repositories
{
    public interface IAchievementRepository
    {
        void Upsert(Achievement achievement);
        Achievement? Get(int achievementId);
        List<Achievement> GetAll();
        List<Achievement> GetByCondition(AchievementCondition conditionType);

        Task CheckForAchievements(
            int userId,
            Methode method,
            List<TransportEntry> entries,
            List<UserChallengeProgress> userChallenges,
            TransportEntry? entry = null
        );
        List<UserAchievement> GetUserAchievements(int id);
        Task<Dictionary<int, int>> GetAchievementProgress(
            int userId,
            List<TransportEntry> entries,
            List<UserChallengeProgress> userChallenges);
    }
}
