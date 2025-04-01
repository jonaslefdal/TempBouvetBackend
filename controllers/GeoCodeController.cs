using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
[Authorize] 
public class GeocodeController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly string _orsApiKey;

    public GeocodeController(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;

        _orsApiKey = configuration["OpenRouteService:ApiKey"];
        if (string.IsNullOrEmpty(_orsApiKey))
        {
            throw new ArgumentNullException("OpenRouteService:ApiKey is not configured");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            return BadRequest("Missing address parameter");
        }

       var orsUrl = $"https://api.openrouteservice.org/geocode/search" +
             $"?api_key={_orsApiKey}" +
             $"&text={Uri.EscapeDataString(address)}" +
             $"&boundary.country=NO" +
             $"&layers=address"; 

        try
        {
            var response = await _httpClient.GetAsync(orsUrl);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Error from ORS service");
            }

            var json = await response.Content.ReadAsStringAsync();

            // Parse and filter features with house numbers
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("features", out var features))
            {
                var filteredFeatures = new System.Text.Json.Nodes.JsonArray();

                foreach (var feature in features.EnumerateArray())
                {
                    if (feature.TryGetProperty("properties", out var props) &&
                        props.TryGetProperty("label", out var labelElement))
                    {
                        var label = labelElement.GetString();
                        if (!string.IsNullOrWhiteSpace(label) && System.Text.RegularExpressions.Regex.IsMatch(label, @"\d"))
                        {
                            filteredFeatures.Add(feature);
                        }
                    }
                }

            var result = new System.Text.Json.Nodes.JsonObject
            {
                ["type"] = System.Text.Json.Nodes.JsonValue.Create(root.GetProperty("type").GetString()),
                ["features"] = filteredFeatures
            };

                return Content(result.ToJsonString(), "application/json");
            }

        return Content(json, "application/json"); 

        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error fetching geocode: {ex.Message}");
        }
    }
}
