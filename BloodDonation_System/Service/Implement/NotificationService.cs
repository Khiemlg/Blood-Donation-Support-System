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

        public async Task<NotificationDto> CreateNotificationAsync(NotificationinputDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Message))
            {
                throw new ArgumentException("Nội dung thông báo không được để trống.", nameof(dto.Message));
            }
            if (string.IsNullOrWhiteSpace(dto.Type))
            {
                throw new ArgumentException("Loại thông báo không được để trống.", nameof(dto.Type));
            }

            var normalizedType = dto.Type.Trim().ToLower();

            if (normalizedType == "chung") 
            {
                if (dto.RecipientUserId == null || dto.RecipientUserId.Trim().ToLower() != "all")
                {
                    throw new ArgumentException("Đối với thông báo 'Chung', Người nhận phải là 'ALL'.", nameof(dto.RecipientUserId));
                }
                dto.RecipientUserId = "ALL"; 
            }
            else if (normalizedType == "don" || normalizedType == "emergency") 
            {
                if (string.IsNullOrWhiteSpace(dto.RecipientUserId) || dto.RecipientUserId.Trim().ToLower() == "all")
                {
                    throw new ArgumentException($"Đối với thông báo loại '{dto.Type}', cần chỉ định Người nhận cụ thể, không thể để trống hoặc là 'ALL'.", nameof(dto.RecipientUserId));
                }
                var recipientExists = await _context.Users.AnyAsync(u => u.UserId == dto.RecipientUserId);
                if (!recipientExists)
                {
                    throw new ArgumentException($"Người nhận với ID '{dto.RecipientUserId}' không tồn tại trong hệ thống.", nameof(dto.RecipientUserId));
                }
            }
            else 
            {
                throw new ArgumentException($"Loại thông báo không hợp lệ: '{dto.Type}'. Các loại hợp lệ: 'Chung', 'Đơn', 'Emergency'.", nameof(dto.Type));
            }

            DateTime finalSentDate = dto.SentDate ?? DateTime.UtcNow;




            var entity = new Notification
            {
                NotificationId = "NOTIF_" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper(), 
                RecipientUserId = dto.RecipientUserId,
                Message = dto.Message,
                Type = dto.Type,
                SentDate = finalSentDate,
                IsRead = false 
            };

            _context.Notifications.Add(entity);
            await _context.SaveChangesAsync();

            return new NotificationDto
            {
                NotificationId = entity.NotificationId,
                RecipientUserId = entity.RecipientUserId,
                Message = entity.Message,
                Type = entity.Type,
                SentDate = entity.SentDate,
                IsRead = entity.IsRead
            };
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

        public async Task<IEnumerable<NotificationDto>> GetNotificationsByRecipientUserIdAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.RecipientUserId == userId || n.RecipientUserId == "ALL")
                .OrderByDescending(n => n.SentDate ?? DateTime.MinValue) 
                .Select(n => new NotificationDto
                {
                    NotificationId = n.NotificationId,
                    RecipientUserId = n.RecipientUserId,
                    Message = n.Message,
                    Type = n.Type,
                    SentDate = n.SentDate,
                    IsRead = n.IsRead
                })
                .ToListAsync();
        }
    }
}