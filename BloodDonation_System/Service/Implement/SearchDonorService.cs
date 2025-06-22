using BloodDonation_System.Data;
using BloodDonation_System.Model.DTO.UserProfile;
using BloodDonation_System.Model.Enties;
using Microsoft.EntityFrameworkCore;
using BloodDonation_System.Service.Interface;

namespace BloodDonation_System.Service.Implement
{
    public class DonorSearchService : ISearchDonorService
    {
        private readonly DButils _context;

        public DonorSearchService(DButils context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserProfileDto>> SearchSuitableDonorsAsync(SearchDonorDto request)
        {
            // Vị trí bệnh viện cố định (ví dụ: Quận 1, TP.HCM)
            decimal hospitalLat = 10.7769m;
            decimal hospitalLon = 106.7009m;

            // Lấy danh sách ứng viên ban đầu
            var candidates = await _context.UserProfiles
                .Include(p => p.User)
                .Where(p =>
                    p.BloodTypeId == request.BloodTypeId &&
                    p.User.IsActive == true &&
                    p.Latitude != null &&
                    p.Longitude != null &&
                    (p.LastBloodDonationDate == null ||
                     p.LastBloodDonationDate.Value.AddDays(90) <= DateOnly.FromDateTime(DateTime.Today))
                )
                .ToListAsync();

            // Lọc theo bán kính
            var filtered = candidates.Where(p =>
            {
                var distance = CalculateDistance(
                    hospitalLat, hospitalLon,
                    p.Latitude!.Value, p.Longitude!.Value
                );
                return distance <= request.RadiusInKm;
            });

            // Trả về kết quả dạng DTO
            var result = filtered.Select(p => new UserProfileDto
            {
                ProfileId = p.ProfileId,
                UserId = p.UserId,
                FullName = p.FullName,
                DateOfBirth = p.DateOfBirth,
                Gender = p.Gender,
                Address = p.Address,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                BloodTypeId = p.BloodTypeId,
                RhFactor = p.RhFactor,
                MedicalHistory = p.MedicalHistory,
                LastBloodDonationDate = p.LastBloodDonationDate,
                Cccd = p.Cccd,
                PhoneNumber = p.PhoneNumber
            });

            return result;
        }

        private double CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            var R = 6371; // Bán kính Trái Đất (km)
            var dLat = ToRadians((double)(lat2 - lat1));
            var dLon = ToRadians((double)(lon2 - lon1));

            var a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians((double)lat1)) * Math.Cos(ToRadians((double)lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}
