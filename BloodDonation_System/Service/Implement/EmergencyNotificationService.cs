using BloodDonation_System.Data;
using BloodDonation_System.Model.DTO.Emergency;
using BloodDonation_System.Model.Enties;
using BloodDonation_System.Service.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BloodDonation_System.Service.Implement
{
    public class EmergencyNotificationService : IEmergencyNotificationService
    {
        private readonly DButils _context;

        public EmergencyNotificationService(DButils context)
        {
            _context = context;
        }
        public async Task<string> RespondToEmergencyNotificationAsync(string userId, EmergencyResponseDTO dto)
        {
            var notification = await _context.EmergencyNotifications
                .FirstOrDefaultAsync(n => n.NotificationId == dto.NotificationId && n.RecipientUserId == userId);

            if (notification == null)
                return "❌ Không tìm thấy thông báo hoặc không thuộc về bạn.";

            // Chỉ chặn nếu đã phản hồi thật sự
            if (notification.ResponseStatus == "Interested" || notification.ResponseStatus == "Declined")
                return "⚠️ Bạn đã phản hồi rồi. Không thể sửa phản hồi.";

            if (dto.ResponseStatus != "Interested" && dto.ResponseStatus != "Declined")
                return "❌ Trạng thái phản hồi không hợp lệ.";

            notification.ResponseStatus = dto.ResponseStatus;
            notification.IsRead = true; // Optional: đánh dấu đã đọc luôn nếu muốn
            await _context.SaveChangesAsync();

            return "✅ Phản hồi đã được ghi nhận.";
        }






        public async Task<IEnumerable<EmergencyNotificationDto>> GetAllAsync()
        {
            return await _context.EmergencyNotifications
                .Select(en => new EmergencyNotificationDto
                {
                    NotificationId = en.NotificationId,
                    EmergencyId = en.EmergencyId,
                    RecipientUserId = en.RecipientUserId,
                    SentDate = en.SentDate,
                    DeliveryMethod = en.DeliveryMethod,
                    IsRead = en.IsRead,
                    Message = en.Message,
                    ResponseStatus = en.ResponseStatus
                }).ToListAsync();
        }

        public async Task<EmergencyNotificationDto?> GetByIdAsync(string notificationId)
        {
            var en = await _context.EmergencyNotifications.FindAsync(notificationId);
            if (en == null) return null;

            return new EmergencyNotificationDto
            {
                NotificationId = en.NotificationId,
                EmergencyId = en.EmergencyId,
                RecipientUserId = en.RecipientUserId,
                SentDate = en.SentDate,
                DeliveryMethod = en.DeliveryMethod,
                IsRead = en.IsRead,
                Message = en.Message,
                ResponseStatus = en.ResponseStatus
            };
        }

        public async Task<EmergencyNotificationDto> CreateAsync(EmergencyNotificationDto dto)
        {
            var entity = new EmergencyNotification
            {
                NotificationId = Guid.NewGuid().ToString(),
                EmergencyId = dto.EmergencyId,
                RecipientUserId = dto.RecipientUserId,
                SentDate = dto.SentDate ?? DateTime.UtcNow,
                DeliveryMethod = dto.DeliveryMethod,
                IsRead = dto.IsRead,
                Message = dto.Message,
                ResponseStatus = dto.ResponseStatus
            };

            _context.EmergencyNotifications.Add(entity);
            await _context.SaveChangesAsync();

            dto.NotificationId = entity.NotificationId;
            return dto;
        }

        public async Task<EmergencyNotificationDto?> UpdateAsync(string notificationId, EmergencyNotificationDto dto)
        {
            var entity = await _context.EmergencyNotifications.FindAsync(notificationId);
            if (entity == null) return null;

            entity.EmergencyId = dto.EmergencyId;
            entity.RecipientUserId = dto.RecipientUserId;
            entity.SentDate = dto.SentDate;
            entity.DeliveryMethod = dto.DeliveryMethod;
            entity.IsRead = dto.IsRead;
            entity.ResponseStatus = dto.ResponseStatus;
            entity.Message = dto.Message;

            _context.EmergencyNotifications.Update(entity);
            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<bool> DeleteAsync(string notificationId)
        {
            var entity = await _context.EmergencyNotifications.FindAsync(notificationId);
            if (entity == null) return false;

            _context.EmergencyNotifications.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task NotifyMatchingMembersAsync(EmergencyRequest request)
        {
            var members = await (from profile in _context.UserProfiles
                                 join user in _context.Users on profile.UserId equals user.UserId
                                 where profile.BloodTypeId == request.BloodTypeId
                                       && user.IsActive == true
                                 select new
                                 {
                                     UserId = user.UserId,
                                     profile.FullName,
                                     profile.BloodTypeId,
                                     profile.Address
                                 }).ToListAsync();

            foreach (var member in members)
            {
                var notification = new EmergencyNotification
                {
                    NotificationId = "EN_" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper(),
                    EmergencyId = request.EmergencyId, // dùng đúng tên khóa chính của bảng EmergencyRequests
                    RecipientUserId = member.UserId,
                    SentDate = DateTime.Now,
                    DeliveryMethod = "App Notification", // hoặc "Email" nếu có xử lý
                    IsRead = false,
                    ResponseStatus = "No Response"
                };

                _context.EmergencyNotifications.Add(notification);
            }

            await _context.SaveChangesAsync();
        }

       

        public async Task<IEnumerable<EmergencyNotificationDto>> GetByEmergencyIdAsync(string emergencyId)
        {
            return await _context.EmergencyNotifications
                .Where(n => n.EmergencyId == emergencyId)
                .Select(en => new EmergencyNotificationDto
                {
                    NotificationId = en.NotificationId,
                    EmergencyId = en.EmergencyId,
                    RecipientUserId = en.RecipientUserId,
                    SentDate = en.SentDate,
                    DeliveryMethod = en.DeliveryMethod,
                    IsRead = en.IsRead,
                    Message = en.Message,
                    ResponseStatus = en.ResponseStatus
                }).ToListAsync();
        }

       
    }
}
