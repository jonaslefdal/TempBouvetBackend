using BouvetBackend.Entities;
using System.Collections.Generic;

namespace BouvetBackend.Repositories
{
    public interface IUserChallengeProgressRepository
    {
        void Upsert(UserChallengeProgress userChallengeProgress);
        UserChallengeProgress Get(int id);
        List<UserChallengeProgress> GetAttemptsByUserId(int UserId);
        List<UserChallengeProgress> GetAll();
        List<UserChallengeProgress> GetAttemptsByUserForChallenge(int UserId, int ChallengeId);
        Task<int> GetUserAttemptCountForChallengeThisWeekAsync(int userId, int challengeId, DateTime weekStart);
        

    }
}
