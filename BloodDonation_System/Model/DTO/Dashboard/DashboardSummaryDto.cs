namespace BloodDonation_System.Model.DTO.Dashboard
{
    public class BloodTypeSummary
    {
        public string BloodType { get; set; } = null!;
        public int TotalUnits { get; set; }
    }

    public class UserStatusSummary
    {
        public string Status { get; set; } = null!;
        public int Count { get; set; }
    }

    public class DonationStat
    {
        public string Month { get; set; } = null!;
        public int Count { get; set; }
    }

    public class DashboardSummaryDto
    {
        public List<BloodTypeSummary> BloodUnitsByType { get; set; } = new();
        public List<DonationStat> DonationsByMonth { get; set; } = new();
        public int EmergencyRequestCount { get; set; }
        public List<UserStatusSummary> UserStatusDistribution { get; set; } = new();
    }
}
