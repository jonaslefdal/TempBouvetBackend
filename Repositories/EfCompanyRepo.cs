using BouvetBackend.DataAccess;
using BouvetBackend.Entities;
using System.Collections.Generic;
using System.Linq;

namespace BouvetBackend.Repositories
{
   public class EfCompanyRepository : ICompanyRepository
{
    private readonly DataContext _context;

    public EfCompanyRepository(DataContext context)
    {
        _context = context;
    }

    public List<Company> GetAll()
    {
        return _context.Company.ToList();
    }

    public Company GetById(int companyId)
    {
        return _context.Company.FirstOrDefault(a => a.CompanyId == companyId);
    }

    public void Upsert(Company company)
    {
        _context.Company.Add(company);
        _context.SaveChanges();
    }
}

}
