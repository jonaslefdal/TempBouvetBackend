using BouvetBackend.Entities;
using System.Collections.Generic;

namespace BouvetBackend.Repositories
{
    public interface IUserRepository
    {
        Users GetUserById(int userId);
        Users GetUserByEmail(string email);
        void InsertOrUpdateUser(Users user);
        List<Users> GetAllUsers();
    }
}
