namespace BloodDonation_System.Service.Interface
{
    public interface IDonationReminderService
    {
        Task RunDonationReminderJobAsync(); // Gọi khi muốn chạy gửi nhắc
    }
}
