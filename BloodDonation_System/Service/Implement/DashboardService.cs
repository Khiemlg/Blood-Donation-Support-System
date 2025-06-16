using BloodDonation_System.Data;
using BloodDonation_System.Model.DTO.Dashboard;
using BloodDonation_System.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation_System.Service.Implement
{
    public class DashboardService : IDashboardService
    {
        private readonly DButils _context;

        public DashboardService(DButils context)
        {
            _context = context;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync()
        {
            var result = new DashboardSummaryDto();

            // Tổng số đơn vị máu theo nhóm máu
            result.BloodUnitsByType = await _context.BloodUnits
                .GroupBy(b => b.BloodType.TypeName)
                .Select(g => new BloodTypeSummary
                {
                    BloodType = g.Key,
                    TotalUnits = g.Count()
                }).ToListAsync();

            // Lượt hiến máu theo tháng (6 tháng gần nhất)
            var now = DateTime.Now;
            var sixMonthsAgo = now.AddMonths(-5);

            result.DonationsByMonth = await _context.DonationHistories
                .Where(d => d.DonationDate >= new DateTime(sixMonthsAgo.Year, sixMonthsAgo.Month, 1))
                .GroupBy(d => new { d.DonationDate.Year, d.DonationDate.Month })
                .Select(g => new DonationStat
                {
                    Month = $"{g.Key.Month}/{g.Key.Year}",
                    Count = g.Count()
                })
                .OrderBy(s => s.Month)
                .ToListAsync();

            // Tổng số yêu cầu khẩn cấp
            result.EmergencyRequestCount = await _context.EmergencyRequests.CountAsync();

            // Phân bố tài khoản (Active vs Inactive)
            result.UserStatusDistribution = await _context.Users
                .GroupBy(u => u.IsActive == true ? "Active" : "Inactive")
                .Select(g => new UserStatusSummary
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return result;
        }
    }
}
