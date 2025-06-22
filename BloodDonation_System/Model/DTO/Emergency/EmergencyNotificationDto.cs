namespace BloodDonation_System.Model.DTO.Emergency
{
    public class EmergencyNotificationDto
    {
        public string NotificationId { get; set; } = null!;
        public string EmergencyId { get; set; } = null!;
        public string RecipientUserId { get; set; } = null!;
        public DateTime? SentDate { get; set; }
        public string DeliveryMethod { get; set; } = null!;
        public bool? IsRead { get; set; }
        public string? ResponseStatus { get; set; }
        public string? Message { get; set; } = null!;

    }
}
