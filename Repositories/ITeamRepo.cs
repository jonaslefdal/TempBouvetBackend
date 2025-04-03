using BouvetBackend.Entities;
using BouvetBackend.Models.TeamModel;
using System.Collections.Generic;

namespace BouvetBackend.Repositories
{
    public interface ITeamRepository
    {
        List<TeamModel> GetTeamsByCompanyId(int companyId);
        Teams? Get(int TeamId);
        void Upsert(Teams team);
        void EditTeam(EditTeamModel team);
        List<Teams> GetAll();
        Teams? GetTeamWithMembers(int teamId);
    }
}
