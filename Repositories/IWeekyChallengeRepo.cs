using System;
using System.Collections.Generic;
using BouvetBackend.Entities;

namespace BouvetBackend.Repositories
{
    public interface IWeeklyChallengeRepository
    {
        // Get the weekly challenges for a given week (by start date)
        List<WeeklyChallenge> GetWeeklyChallenges(DateTime weekStartDate);
        
        // Insert a list of weekly challenges (for seeding the week)
        void InsertWeeklyChallenges(List<WeeklyChallenge> weeklyChallenges);
    }
}
