using BouvetBackend.Entities;
using BouvetBackend.DataAccess;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace BouvetBackend.Repositories
{
    public class EfUserChallengeProgressRepository : IUserChallengeProgressRepository
    {
        private readonly DataContext _context;

        public EfUserChallengeProgressRepository(DataContext context)
        {
            _context = context;
        }

        // Inserts a new record or updates an existing one
        public void Upsert(UserChallengeProgress userChallengeProgress)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userChallengeProgress.UserId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Always insert new attempt
            _context.UserChallengeProgress.Add(userChallengeProgress);
            user.TotalScore += userChallengeProgress.PointsAwarded;

            _context.SaveChanges();
        }

        // Retrieves a specific attempt by its ID
        public UserChallengeProgress? Get(int id)
        {
            return _context.UserChallengeProgress.FirstOrDefault(a => a.UserChallengeProgressId == id);
        }

        // Retrieves all user challenge attempts
        public List<UserChallengeProgress> GetAll()
        {
            return _context.UserChallengeProgress.ToList();
        }

        // Retrieves attempts for a specific user and challenge
        public List<UserChallengeProgress> GetAttemptsByUserForChallenge(int userId, int challengeId)
        {
            return _context.UserChallengeProgress
                .Where(a => a.UserId == userId && a.ChallengeId == challengeId)
                .ToList();
        }

        public List<UserChallengeProgress> GetAttemptsByUserId(int userId)
        {
            return _context.UserChallengeProgress
                        .Where(a => a.UserId == userId)
                        .ToList();
        }

        public async Task<int> GetUserAttemptCountForChallengeThisWeekAsync(int userId, int challengeId, DateTime weekStart)
        {
            return await _context.UserChallengeProgress.CountAsync(a =>
                a.UserId == userId &&
                a.ChallengeId == challengeId &&
                a.AttemptedAt >= weekStart);
        }

    }
}
