using BouvetBackend.Entities;

namespace BouvetBackend.Repositories
{
    public interface ICompanyRepository
    {
        void Upsert(Company Company);
        Company? GetById(int CompanyId);
        List<Company> GetAll();
        List<object> GetCompanyScores();
    }
}
