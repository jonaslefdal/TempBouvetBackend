using BouvetBackend.Entities;
using BouvetBackend.DataAccess;
using System.Collections.Generic;
using System.Linq;

namespace BouvetBackend.Repositories
{
    public class EfTransportEntryRepository : ITransportEntryRepository
    {
        private readonly DataContext _context;

        public EfTransportEntryRepository(DataContext context)
        {
            _context = context;
        }

       public void Upsert(TransportEntry transportEntry)
{
    // Retrieve the associated user
    var user = _context.Users.FirstOrDefault(u => u.UserId == transportEntry.UserId);
    if (user == null)
    {
        throw new Exception("User not found.");
    }

    var existing = _context.TransportEntry
        .FirstOrDefault(a => a.TransportEntryId == transportEntry.TransportEntryId);

    if (existing != null)
    {
        int pointDifference = transportEntry.Points - existing.Points;

        existing.Method = transportEntry.Method;
        existing.Points = transportEntry.Points;
        existing.CreatedAt = transportEntry.CreatedAt;
        
        user.TotalScore += pointDifference;
    }
    else
    {
        _context.TransportEntry.Add(transportEntry);
        user.TotalScore += transportEntry.Points;
    }

    _context.SaveChanges();
}


        public TransportEntry Get(int TransportEntryId)
        {
            return _context.TransportEntry.FirstOrDefault(a => a.TransportEntryId == TransportEntryId);
        }

        public List<TransportEntry> GetAll()
        {
            return _context.TransportEntry.ToList();
        }
    }
}
