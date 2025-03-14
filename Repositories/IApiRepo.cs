//EXAMPLE refrence

using BouvetBackend.Entities;

namespace BouvetBackend.Repositories
{
    public interface IApiRepository
    {
        void Upsert(API api);
        API Get(int apiId);
        List<API> GetAll();
        
    }
}
