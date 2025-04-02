using BouvetBackend.Entities;
using BouvetBackend.DataAccess;
using System.Collections.Generic;
using System.Linq;

namespace BouvetBackend.Repositories
{
    public class EfChallengeRepository : IChallengeRepository
    {
        private readonly DataContext _context;

        public EfChallengeRepository(DataContext context)
        {
            _context = context;
        }

       public void Upsert(Challenge challenge)
        {
            var existing = _context.Challenge
                .FirstOrDefault(a => a.ChallengeId == challenge.ChallengeId);

            if (existing != null)
            {
                existing.ChallengeId = challenge.ChallengeId;
                existing.Description = challenge.Description;
                existing.Points = challenge.Points;
                existing.MaxAttempts = challenge.MaxAttempts;
            }
            else
            {
                _context.Challenge.Add(challenge);
            }

            _context.SaveChanges();
        }
        public List<Challenge> GetChallengesByRotationGroup(int rotationGroup)
        {
            return _context.Challenge
                        .Where(c => c.RotationGroup == rotationGroup)
                        .ToList();
        }
        public Challenge Get(int ChallengeId)
        {
            var challenge = _context.Challenge.FirstOrDefault(a => a.ChallengeId == ChallengeId);
            if (challenge == null)
            {
                throw new KeyNotFoundException("ChallengeKey Null");
            }
            return challenge;
        }

        public List<Challenge> GetAll()
        {
            return _context.Challenge.ToList();
        }
    }
}
