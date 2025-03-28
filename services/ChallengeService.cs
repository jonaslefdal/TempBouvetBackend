using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BouvetBackend.DataAccess;
using BouvetBackend.Entities;
using BouvetBackend.Extensions;

namespace BouvetBackend.Services
{
    public class ChallengeProgressService
    {
        private readonly DataContext _context;
    
        public ChallengeProgressService(DataContext context)
        {
            _context = context;
        }
    
        public async Task CheckAndUpdateProgress(int userId, string method)
        {
            DateTime weekStart = DateTime.UtcNow.StartOfWeek(DayOfWeek.Monday);

            var entriesThisWeek = await _context.TransportEntry
                .Where(te => te.UserId == userId && te.Method == method && te.CreatedAt >= weekStart)
                .ToListAsync();

            int countThisWeek = entriesThisWeek.Count;
            Console.WriteLine($"DEBUG: User {userId}, Method: {method}, Entries this week: {countThisWeek}");

            int currentGroup = GetCurrentRotationGroup();
            Console.WriteLine($"DEBUG: Current RotationGroup: {currentGroup}");

            var challenges = await _context.Challenge
                .Where(c => c.RotationGroup == currentGroup && c.RequiredTransportMethod == method)
                .ToListAsync();

            Console.WriteLine($"DEBUG: Challenges matching: {challenges.Count}");

            foreach (var challenge in challenges)
            {
                Console.WriteLine($"DEBUG: Challenge {challenge.ChallengeId}, RequiredCount: {challenge.MaxAttempts}");

            if (challenge.ConditionType == "Distance" && challenge.RequiredDistanceKm.HasValue)
            {
                double distanceThisWeek = entriesThisWeek.Sum(e => e.DistanceKm);
                if (distanceThisWeek >= challenge.RequiredDistanceKm.Value)
                {
                    bool alreadyCompleted = await _context.UserChallengeProgress.AnyAsync(a =>
                        a.UserId == userId &&
                        a.ChallengeId == challenge.ChallengeId &&
                        a.AttemptedAt >= weekStart);

                    if (!alreadyCompleted)
                    {
                        await CompleteChallengeForUser(userId, challenge);
                    }
                }
            }
            else if (challenge.MaxAttempts.HasValue && countThisWeek >= challenge.MaxAttempts.Value)
            {
                bool alreadyCompleted = await _context.UserChallengeProgress.AnyAsync(a =>
                    a.UserId == userId &&
                    a.ChallengeId == challenge.ChallengeId &&
                    a.AttemptedAt >= weekStart);

                if (!alreadyCompleted)
                {
                    await CompleteChallengeForUser(userId, challenge);
                }
            }

                else
                {
                    Console.WriteLine($"DEBUG: Challenge {challenge.ChallengeId} not yet eligible.");
                }
            }
        }

        private int GetCurrentRotationGroup()
        {
            int totalGroups = 10;
            DateTime cycleStart = new DateTime(2025, 3, 13);
            int weeksSinceStart = (int)((DateTime.UtcNow - cycleStart).TotalDays / 7);
            return (weeksSinceStart % totalGroups) + 1;
        }

       private async Task CompleteChallengeForUser(int userId, Challenge challenge)
        {
            var attempt = new UserChallengeProgress
            {
                UserId = userId,
                ChallengeId = challenge.ChallengeId,
                PointsAwarded = challenge.Points,
                AttemptedAt = DateTime.UtcNow
            };

            await _context.UserChallengeProgress.AddAsync(attempt);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user != null)
            {
                user.TotalScore += challenge.Points;
            }

            await _context.SaveChangesAsync();
        }

    }
}
