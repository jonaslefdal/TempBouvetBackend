using System.Collections.Generic;
using System.Linq;
using BouvetBackend.DataAccess;
using BouvetBackend.Entities;
using BouvetBackend.Models.TeamModel;
using Microsoft.EntityFrameworkCore;

namespace BouvetBackend.Repositories
{
    public class EfTeamRepository : ITeamRepository
    {
        private readonly DataContext _context;

        public EfTeamRepository(DataContext context)
        {
            _context = context;
        }

        public List<TeamModel> GetTeamsByCompanyId(int companyId)
        {
            return _context.Teams
                .Where(t => t.CompanyId == companyId)
                .Include(t => t.Users) 
                .Select(t => new TeamModel
                {
                    TeamId = t.TeamId,
                    Name = t.Name,
                    CompanyId = t.CompanyId,
                    MaxMembers = t.MaxMembers,
                    MemberCount = t.Users.Count()
                })
                .ToList();
        }


        public Teams Get(int teamId)
        {
            return _context.Teams.FirstOrDefault(t => t.TeamId == teamId);
        }

        public void Upsert(Teams team)
        {
            var existing = _context.Teams.FirstOrDefault(t => t.TeamId == team.TeamId);
            if (existing != null)
            {
                existing.Name = team.Name;
                existing.CompanyId = team.CompanyId;
            }
            else
            {
                _context.Teams.Add(team);
            }
            _context.SaveChanges();
        }
        
        public List<Teams> GetAll()
        {
            return _context.Teams.ToList();
        }
        
        public Teams GetTeamWithMembers(int teamId)
        {
            return _context.Teams
                        .Where(t => t.TeamId == teamId)
                        .Include(t => t.Users) 
                        .FirstOrDefault();
        }

    }
}
