using BouvetBackend.Entities;

namespace BouvetBackend.Repositories
{
    public interface IChallengeRepository
    {
        void Upsert(Challenge Challenge);
        Challenge Get(int ChallengeId);
        List<Challenge> GetAll();
        List<Challenge> GetChallengesByRotationGroup(int rotationGroup);
        
    }
}
