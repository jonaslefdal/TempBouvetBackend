using BouvetBackend.Entities;
using System.Collections.Generic;

namespace BouvetBackend.Repositories
{
    public interface IUserChallengeAttemptRepository
    {
        void Upsert(UserChallengeAttempt userChallengeAttempt);
        UserChallengeAttempt Get(int id);
        List<UserChallengeAttempt> GetAttemptsByUserId(int UserId);
        List<UserChallengeAttempt> GetAll();
        List<UserChallengeAttempt> GetAttemptsByUserForChallenge(int UserId, int ChallengeId);
    }
}
