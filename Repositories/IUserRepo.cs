using BouvetBackend.Entities;
using System.Collections.Generic;

namespace BouvetBackend.Repositories
{
    public interface IUserRepository
    {
        Users GetUserById(int userId);
        Users GetUserByEmail(string email);
        Users GetUserByAzureId(string azureId);
        void InsertOrUpdateUser(Users user);
        void UpdateUserProfile(Users user);
        List<Users> GetAllUsers();
    }
}
