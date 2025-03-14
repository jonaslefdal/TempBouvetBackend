using BouvetBackend.Entities;
using BouvetBackend.DataAccess;
using System.Collections.Generic;
using System.Linq;

namespace BouvetBackend.Repositories
{
    public class EfUserChallengeAttemptRepository : IUserChallengeAttemptRepository
    {
        private readonly DataContext _context;

        public EfUserChallengeAttemptRepository(DataContext context)
        {
            _context = context;
        }

        // Inserts a new record or updates an existing one
        public void Upsert(UserChallengeAttempt userChallengeAttempt)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userChallengeAttempt.UserId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var existing = _context.UserChallengeAttempt
                .FirstOrDefault(a => a.UserChallengeAttemptId == userChallengeAttempt.UserChallengeAttemptId);
            if (existing != null)
            {

                int pointDifference = userChallengeAttempt.PointsAwarded - existing.PointsAwarded;

                existing.UserId = userChallengeAttempt.UserId;
                existing.ChallengeId = userChallengeAttempt.ChallengeId;
                existing.PointsAwarded = userChallengeAttempt.PointsAwarded;
                existing.AttemptedAt = userChallengeAttempt.AttemptedAt;

                user.TotalScore += pointDifference;
            }
            else
            {
                _context.UserChallengeAttempt.Add(userChallengeAttempt);
                user.TotalScore += userChallengeAttempt.PointsAwarded;
            }

            _context.SaveChanges();
        }

        // Retrieves a specific attempt by its ID
        public UserChallengeAttempt Get(int id)
        {
            return _context.UserChallengeAttempt.FirstOrDefault(a => a.UserChallengeAttemptId == id);
        }

        // Retrieves all user challenge attempts
        public List<UserChallengeAttempt> GetAll()
        {
            return _context.UserChallengeAttempt.ToList();
        }

        // Retrieves attempts for a specific user and challenge
        public List<UserChallengeAttempt> GetAttemptsByUserForChallenge(int userId, int challengeId)
        {
            return _context.UserChallengeAttempt
                .Where(a => a.UserId == userId && a.ChallengeId == challengeId)
                .ToList();
        }

        public List<UserChallengeAttempt> GetAttemptsByUserId(int userId)
        {
            return _context.UserChallengeAttempt
                        .Where(a => a.UserId == userId)
                        .ToList();
        }

    }
}
