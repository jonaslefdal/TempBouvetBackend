using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BouvetBackend.DataAccess;
using BouvetBackend.Entities;

namespace BouvetBackend.Services
{
    public static class DateTimeExtensions
{
    public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
    {
        int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
        return dt.Date.AddDays(-1 * diff).Date;
    }
}
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

        int currentGroup = GetCurrentRotationGroup();

        var challenges = await _context.Challenge
            .Where(c => c.RotationGroup == currentGroup && c.RequiredTransportMethod == method)
            .ToListAsync();

        foreach (var challenge in challenges)
        {
            if (challenge.RequiredCount.HasValue && countThisWeek >= challenge.RequiredCount.Value)
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
            var attempt = new UserChallengeAttempt
            {
                UserId = userId,
                ChallengeId = challenge.ChallengeId,
                PointsAwarded = challenge.Points,
                AttemptedAt = DateTime.UtcNow
            };

            await _context.UserChallengeAttempt.AddAsync(attempt);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user != null)
            {
                user.TotalScore += challenge.Points;
            }

            await _context.SaveChangesAsync();
        }

    }
}
