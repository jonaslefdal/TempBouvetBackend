using System.Threading.Tasks;

namespace BouvetBackend.Services
{
    public interface IDistanceService
    {
        /// Gets the distance (in kilometers) between two coordinate points.
        Task<double> GetDistance(double[] origin, double[] destination);
    }
}
