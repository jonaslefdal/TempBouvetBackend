using BouvetBackend.Entities;

namespace BouvetBackend.Repositories
{
    public interface IEndUserAddressRepository
    {
        void Upsert(EndUserAddress endUserAddress);
        EndUserAddress? GetUserEndAddress(int userId);
        List<EndUserAddress> GetAll();
    }
}
