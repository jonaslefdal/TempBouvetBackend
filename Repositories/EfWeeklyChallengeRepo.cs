using System;
using System.Collections.Generic;
using System.Linq;
using BouvetBackend.DataAccess;
using BouvetBackend.Entities;

namespace BouvetBackend.Repositories
{
    public class EfWeeklyChallengeRepository : IWeeklyChallengeRepository
    {
        private readonly DataContext _context;

        public EfWeeklyChallengeRepository(DataContext context)
        {
            _context = context;
        }

        public List<WeeklyChallenge> GetWeeklyChallenges(DateTime weekStartDate)
        {
            return _context.WeeklyChallenge
                .Where(wc => wc.WeekStartDate.Date == weekStartDate.Date)
                .OrderBy(wc => wc.DisplayOrder)
                .ToList();
        }

        public void InsertWeeklyChallenges(List<WeeklyChallenge> weeklyChallenges)
        {
            _context.WeeklyChallenge.AddRange(weeklyChallenges);
            _context.SaveChanges();
        }
    }
}
