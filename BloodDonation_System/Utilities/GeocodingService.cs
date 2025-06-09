using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BloodDonation_System.Utilities
{
    public class GeocodingService
    {
        private readonly HttpClient _httpClient;

        public GeocodingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(decimal lat, decimal lon)> GetCoordinatesFromAddressAsync(string? address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return (0, 0);

            string encoded = Uri.EscapeDataString(address);
            string url = $"https://nominatim.openstreetmap.org/search?q={encoded}&format=json&limit=1";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return (0, 0);

            string json = await response.Content.ReadAsStringAsync();
            var root = JsonDocument.Parse(json).RootElement;

            if (root.GetArrayLength() == 0)
                return (0, 0);

            var first = root[0];
            decimal lat = decimal.Parse(first.GetProperty("lat").GetString() ?? "0");
            decimal lon = decimal.Parse(first.GetProperty("lon").GetString() ?? "0");

            return (lat, lon);
        }
    }
}
