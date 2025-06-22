//using System.Globalization;
//using System.Net.Http;
//using System.Text.Json;
//using System.Threading.Tasks;

//namespace BloodDonation_System.Utilities
//{
//    public class GeocodingService
//    {
//        private readonly HttpClient _httpClient;


//        public GeocodingService(HttpClient httpClient)
//        {
//            _httpClient = httpClient;



//            // Thêm User-Agent theo yêu cầu của Nominatim
//            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("BloodDonationSystem/1.0 (your-email@example.com)");
//        }

//        public async Task<(decimal lat, decimal lon)> GetCoordinatesFromAddressAsync(string? address)
//        {
//            if (string.IsNullOrWhiteSpace(address))
//                return (0, 0);

//            // Nếu thiếu tỉnh hoặc quốc gia, thêm vào để tăng độ chính xác
//            if (!address.Contains("Vietnam", StringComparison.OrdinalIgnoreCase))
//            {
//                address += ", Vietnam";
//            }

//            string encoded = Uri.EscapeDataString(address);
//            string url = $"https://nominatim.openstreetmap.org/search?q={encoded}&format=json&limit=1";

//            var response = await _httpClient.GetAsync(url);
//            if (!response.IsSuccessStatusCode)
//                return (0, 0);

//            string json = await response.Content.ReadAsStringAsync();
//            var root = JsonDocument.Parse(json).RootElement;

//            if (root.GetArrayLength() == 0)
//                return (0, 0);

//            var first = root[0];
//            string? latStr = first.GetProperty("lat").GetString();
//            string? lonStr = first.GetProperty("lon").GetString();

//            decimal.TryParse(latStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal lat);
//            decimal.TryParse(lonStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal lon);

//            return (lat, lon);
//        }
//    }
//}
using System;
using System.Globalization;
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

            // ✅ Cập nhật User-Agent đúng chuẩn để tránh bị Nominatim từ chối
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("BloodDonationApp/1.0 (contact@yourdomain.com)");
        }

        public async Task<(decimal lat, decimal lon)> GetCoordinatesFromAddressAsync(string? address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return (0, 0);

            // ✅ Nếu chưa có từ "Vietnam" thì thêm để tăng độ chính xác
            if (!address.Contains("Vietnam", StringComparison.OrdinalIgnoreCase))
            {
                address += ", Vietnam";
            }

            string encoded = Uri.EscapeDataString(address);
            string url = $"https://nominatim.openstreetmap.org/search?q={encoded}&format=json&limit=1";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[GeocodingService] ❌ HTTP failed: {response.StatusCode}");
                return (0, 0);
            }

            string json = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[GeocodingService] ✅ Raw response: {json}");

            var root = JsonDocument.Parse(json).RootElement;
            if (root.GetArrayLength() == 0)
            {
                Console.WriteLine("[GeocodingService] ⚠️ No results returned.");
                return (0, 0);
            }

            var first = root[0];
            string? latStr = first.GetProperty("lat").GetString();
            string? lonStr = first.GetProperty("lon").GetString();

            decimal.TryParse(latStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal lat);
            decimal.TryParse(lonStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal lon);

            Console.WriteLine($"[GeocodingService] ✅ Parsed lat/lon: {lat}, {lon}");
            return (lat, lon);
        }
    }
}

