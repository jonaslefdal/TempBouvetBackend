//EXAMPLE refrence

using BouvetBackend.Entities;
using BouvetBackend.DataAccess;
using System.Collections.Generic;
using System.Linq;

namespace BouvetBackend.Repositories
{
    public class EfEndUserAddressRepository : IEndUserAddressRepository
    {
        private readonly DataContext _context;

        public EfEndUserAddressRepository(DataContext context)
        {
            _context = context;
        }

        public void Upsert(EndUserAddress endUserAddress)
        {
            var existing = _context.EndUserAddress
            .SingleOrDefault(a => a.UserId == endUserAddress.UserId);
            if (existing == null)
            {
                _context.EndUserAddress.Add(endUserAddress);
            }
            else if (!string.Equals(
                     existing.EndAddress,
                     endUserAddress.EndAddress,
                     StringComparison.OrdinalIgnoreCase))
            {
                existing.EndAddress = endUserAddress.EndAddress;
            }
            else
            {
                return;
            }

            _context.SaveChanges();
        }

        public EndUserAddress? GetUserEndAddress(int userId)
        {
            return _context.EndUserAddress
                .FirstOrDefault(a => a.UserId == userId);
        }

        public List<EndUserAddress> GetAll()
        {
            return _context.EndUserAddress.ToList();
        }
    }
}
