namespace BloodDonation_System.Model.Enties
{
    public class ReminderLog
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ReminderType { get; set; }    
        public DateTime SentAt { get; set; }
        public string Via { get; set; }              
    }
}
