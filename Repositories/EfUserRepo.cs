using BouvetBackend.DataAccess;
using BouvetBackend.Entities;
using System.Collections.Generic;
using System.Linq;

namespace BouvetBackend.Repositories
{
    public class EfUserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public EfUserRepository(DataContext context)
        {
            _context = context;
        }

        public Users GetUserById(int userId)
        {
            return _context.Users.FirstOrDefault(u => u.UserId == userId);
        }

        public Users GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public void InsertOrUpdateUser(Users user)
        {
            var existing = _context.Users.FirstOrDefault(u => u.Email == user.Email);
            if (existing != null)
            {
                existing.AzureId = user.AzureId;
                existing.Email = user.Email;
                existing.Name = user.Name;
            }
            else
            {
                _context.Users.Add(user);
            }
            _context.SaveChanges();
        }

        public List<Users> GetAllUsers()
        {
            return _context.Users.OrderByDescending(u => u.TotalScore).ToList();
        }
    }
}
