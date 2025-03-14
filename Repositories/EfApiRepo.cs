//EXAMPLE refrence

using BouvetBackend.Entities;
using BouvetBackend.DataAccess;
using System.Collections.Generic;
using System.Linq;

namespace BouvetBackend.Repositories
{
    public class EfApiRepository : IApiRepository
    {
        private readonly DataContext _context;

        public EfApiRepository(DataContext context)
        {
            _context = context;
        }

        public void Upsert(API api)
        {
            var existing = _context.API.FirstOrDefault(a => a.apiId == api.apiId);
            if (existing != null)
            {
                existing.value1 = api.value1;
                existing.value2 = api.value2;
            }
            else
            {
                _context.API.Add(api);
            }

            _context.SaveChanges();
        }

        public API Get(int apiId)
        {
            return _context.API.FirstOrDefault(a => a.apiId == apiId);
        }

        public List<API> GetAll()
        {
            return _context.API.ToList();
        }
    }
}
