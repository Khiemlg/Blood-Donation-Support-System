using BloodDonation_System.Model.DTO.Dashboard;

namespace BloodDonation_System.Service.Interface
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetSummaryAsync();
    }
}
