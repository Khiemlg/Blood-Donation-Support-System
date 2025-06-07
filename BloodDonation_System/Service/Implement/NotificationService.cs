using BloodDonation_System.Data;
using BloodDonation_System.Model.DTO.Notification;
using BloodDonation_System.Model.Enties;
using BloodDonation_System.Service.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BloodDonation_System.Service.Implement
{
    public class NotificationService : INotificationService
    {
        private readonly DButils _context;

        public NotificationService(DButils context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NotificationDto>> GetAllNotificationsAsync()
        {
            return await _context.Notifications
                .Select(n => new NotificationDto
                {
                    NotificationId = n.NotificationId,
                    RecipientUserId = n.RecipientUserId,
                    Message = n.Message,
                    Type = n.Type,
                    SentDate = n.SentDate,
                    IsRead = n.IsRead
                }).ToListAsync();
        }

        public async Task<NotificationDto?> GetNotificationByIdAsync(string notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null) return null;

            return new NotificationDto
            {
                NotificationId = notification.NotificationId,
                RecipientUserId = notification.RecipientUserId,
                Message = notification.Message,
                Type = notification.Type,
                SentDate = notification.SentDate,
                IsRead = notification.IsRead
            };
        }

        public async Task<NotificationDto> CreateNotificationAsync(NotificationDto notificationDto)
        {
            var entity = new Notification
            {
                NotificationId = Guid.NewGuid().ToString(),
                RecipientUserId = notificationDto.RecipientUserId,
                Message = notificationDto.Message,
                Type = notificationDto.Type,
                SentDate = notificationDto.SentDate ?? DateTime.UtcNow,
                IsRead = notificationDto.IsRead ?? false
            };

            _context.Notifications.Add(entity);
            await _context.SaveChangesAsync();

            notificationDto.NotificationId = entity.NotificationId;
            return notificationDto;
        }

        public async Task<NotificationDto?> UpdateNotificationAsync(string notificationId, NotificationDto notificationDto)
        {
            var entity = await _context.Notifications.FindAsync(notificationId);
            if (entity == null) return null;

            entity.Message = notificationDto.Message;
            entity.Type = notificationDto.Type;
            entity.SentDate = notificationDto.SentDate;
            entity.IsRead = notificationDto.IsRead;

            _context.Notifications.Update(entity);
            await _context.SaveChangesAsync();

            return notificationDto;
        }

        public async Task<bool> DeleteNotificationAsync(string notificationId)
        {
            var entity = await _context.Notifications.FindAsync(notificationId);
            if (entity == null) return false;

            _context.Notifications.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
