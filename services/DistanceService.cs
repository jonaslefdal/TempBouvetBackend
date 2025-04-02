using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace BouvetBackend.Services
{
    public class DistanceService : IDistanceService
    {
        private readonly HttpClient _httpClient;
        private readonly string _orsApiKey;

        public DistanceService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _orsApiKey = configuration["OpenRouteService:ApiKey"]!;
            if (string.IsNullOrEmpty(_orsApiKey))
            {
                throw new ArgumentNullException("OpenRouteService:ApiKey is not configured");
            }
        }

        public async Task<double> GetDistance(double[] origin, double[] destination)
        {
            var url = "https://api.openrouteservice.org/v2/directions/driving-car";

            // Create the request body with the coordinates:
            var requestBody = new
            {
                coordinates = new[] { origin, destination }
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            // Create a new HttpRequestMessage, add the API key in the headers.
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Authorization", _orsApiKey);
            request.Content = jsonContent;

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error fetching distance: {response.ReasonPhrase}");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var directionsResponse = JsonSerializer.Deserialize<ORSRouteResponse>(responseString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (directionsResponse?.Routes != null && directionsResponse.Routes.Length > 0)
            {
                // The distance is returned in meters.
                double distanceMeters = directionsResponse.Routes[0].Summary.Distance;
                return distanceMeters / 1000.0; // Convert meters to kilometers.
            }

            throw new Exception("No routes found in the response.");
        }
    }

    // DTO classes to match the ORS Directions API response structure.
    public class ORSRouteResponse
    {
        public required ORSRoute[] Routes { get; set; }
    }

    public class ORSRoute
    {
        public required ORSSummary Summary { get; set; }
    }

    public class ORSSummary
    {
        public double Distance { get; set; }
    }
}
