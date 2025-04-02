using System.Threading.Tasks;

namespace BouvetBackend.Services
{
    public interface IGeocodingService
    {
        // Gets coordinates [longitude, latitude] for a given address.
        Task<double[]?> GetCoordinates(string address);
    }
}
