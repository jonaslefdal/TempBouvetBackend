using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace BouvetBackend.Services
{
    public class GeocodingService : IGeocodingService
    {
        private readonly HttpClient _httpClient;
        private readonly string _orsApiKey;

        public GeocodingService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _orsApiKey = configuration["OpenRouteService:ApiKey"];
            if (string.IsNullOrEmpty(_orsApiKey))
            {
                throw new ArgumentNullException("OpenRouteService:ApiKey is not configured");
            }
        }

        public async Task<double[]> GetCoordinates(string address)
        {
            var url = $"https://api.openrouteservice.org/geocode/search?api_key={_orsApiKey}&text={Uri.EscapeDataString(address)}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var jsonString = await response.Content.ReadAsStringAsync();

            var geocodeResponse = JsonSerializer.Deserialize<ORSGeocodeResponse>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (geocodeResponse?.Features != null && geocodeResponse.Features.Length > 0)
            {
                return geocodeResponse.Features[0].Geometry.Coordinates;
            }

            return null;
        }
    }

    // DTO classes to match the ORS Geocoding API response.
    public class ORSGeocodeResponse
    {
        public ORSFeature[] Features { get; set; }
    }

    public class ORSFeature
    {
        public ORSGeometry Geometry { get; set; }
    }

    public class ORSGeometry
    {
        // Expected to be [longitude, latitude]
        public double[] Coordinates { get; set; }
    }
}
