﻿namespace BloodDonation_System.Model.DTO.Notification
{
    public class NotificationinputDto
    {
        public string RecipientUserId { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string? Type { get; set; }
        public DateTime? SentDate { get; set; }
        public bool? IsRead { get; set; }
    }
}
