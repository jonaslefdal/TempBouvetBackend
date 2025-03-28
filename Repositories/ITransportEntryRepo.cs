using BouvetBackend.Entities;

namespace BouvetBackend.Repositories
{
    public interface ITransportEntryRepository
    {
        void Upsert(TransportEntry TransportEntry);
        TransportEntry Get(int TransportEntryId);
        List<TransportEntry> GetAll();
        double GetTotalCo2SavingsByUser(int userId);
        int GetTotalTravelCountByUser(int userId); 
        double GetTotalMoneySaved(int userId); 
        int GetTransportEntryCount(int userId, string method, DateTime since);
        int GetTransportDistanceCount(int userId, string method, DateTime since, double requiredDistanceKm);

        double GetTransportDistanceSum(int userId, string method, DateTime since);
        
    }
}
