namespace BloodDonation_System.Model.Enties
{
    public class ReminderLog
    {
        public int Id { get; set; }
        public string UserId { get; set; }           // FK tới bảng Users
        public string ReminderType { get; set; }     // ví dụ: 'BloodDonation'
        public DateTime SentAt { get; set; }
        public string Via { get; set; }              // 'Notification', 'Email', 'Both'
    }
}
