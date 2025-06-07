using BloodDonation_System.Model.DTO.Notification;

namespace BloodDonation_System.Service.Interface
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDto>> GetAllNotificationsAsync();
        Task<NotificationDto?> GetNotificationByIdAsync(string notificationId);
        Task<NotificationDto> CreateNotificationAsync(NotificationDto notificationDto);
        Task<NotificationDto?> UpdateNotificationAsync(string notificationId, NotificationDto notificationDto);
        Task<bool> DeleteNotificationAsync(string notificationId);
    }
}
