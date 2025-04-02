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
    
        public async Task CheckAndUpdateProgress(int userId, Methode method, List<TransportEntry> entriesThisWeek, List<UserChallengeProgress> attemptsThisWeek)
    {
        DateTime weekStart = DateTime.UtcNow.StartOfWeek(DayOfWeek.Monday);

        int countThisWeek = entriesThisWeek.Count(e => e.CreatedAt >= weekStart);
        double distanceThisWeek = entriesThisWeek.Where(e => e.CreatedAt >= weekStart).Sum(e => e.DistanceKm);

        int currentGroup = GetCurrentRotationGroup();

        var challenges = await _context.Challenge
            .Where(c => c.RotationGroup == currentGroup && c.RequiredTransportMethod == method)
            .ToListAsync();

        foreach (var challenge in challenges)
        {
            bool alreadyCompleted = attemptsThisWeek.Any(a =>
                a.UserId == userId &&
                a.ChallengeId == challenge.ChallengeId &&
                a.AttemptedAt >= weekStart);

            if (alreadyCompleted)
                continue;

            if (challenge.ConditionType == "Distance" && challenge.RequiredDistanceKm.HasValue)
            {
                if (distanceThisWeek >= challenge.RequiredDistanceKm.Value)
                    await CompleteChallengeForUser(userId, challenge);
            }
            else if (challenge.MaxAttempts.HasValue && countThisWeek >= challenge.MaxAttempts.Value)
            {
                await CompleteChallengeForUser(userId, challenge);
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
