using BloodDonation_System.Data; // DbContext
using BloodDonation_System.Model.DTO.Notification;
using BloodDonation_System.Model.Enties; // Entity
using BloodDonation_System.Service.Interface;
using Microsoft.EntityFrameworkCore; // Cần thiết cho .AnyAsync()
using System;
using System.Collections.Generic;
using System.Linq; // Cần thiết cho .AnyAsync()
using System.Threading.Tasks;

namespace BloodDonation_System.Service.Implement
{
    public class NotificationService : INotificationService
    {
        private readonly DButils _context; // DbContext của bạn

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

        // SỬA ĐỔI: Chữ ký nhận NotificationInputDto
        public async Task<NotificationDto> CreateNotificationAsync(NotificationinputDto dto)
        {
            // === VALIDATION NGHIỆP VỤ (SERVER-SIDE) ===
            if (string.IsNullOrWhiteSpace(dto.Message))
            {
                throw new ArgumentException("Nội dung thông báo không được để trống.", nameof(dto.Message));
            }
            if (string.IsNullOrWhiteSpace(dto.Type))
            {
                throw new ArgumentException("Loại thông báo không được để trống.", nameof(dto.Type));
            }

            var normalizedType = dto.Type.Trim().ToLower();

            // Logic kiểm tra RecipientUserId dựa trên Type
            if (normalizedType == "chung") // Loại "Chung" (gửi tới tất cả)
            {
                // Đảm bảo RecipientUserId là "ALL" (không phân biệt hoa/thường)
                if (dto.RecipientUserId == null || dto.RecipientUserId.Trim().ToLower() != "all")
                {
                    throw new ArgumentException("Đối với thông báo 'Chung', Người nhận phải là 'ALL'.", nameof(dto.RecipientUserId));
                }
                dto.RecipientUserId = "ALL"; // Đảm bảo lưu đúng "ALL"
            }
            else if (normalizedType == "don" || normalizedType == "emergency") // Loại "Đơn" hoặc "Khẩn cấp"
            {
                // Cần một RecipientUserId cụ thể và không phải là "ALL"
                if (string.IsNullOrWhiteSpace(dto.RecipientUserId) || dto.RecipientUserId.Trim().ToLower() == "all")
                {
                    throw new ArgumentException($"Đối với thông báo loại '{dto.Type}', cần chỉ định Người nhận cụ thể, không thể để trống hoặc là 'ALL'.", nameof(dto.RecipientUserId));
                }
                // OPTIONAL: Kiểm tra xem RecipientUserId có tồn tại trong bảng User không
                var recipientExists = await _context.Users.AnyAsync(u => u.UserId == dto.RecipientUserId);
                if (!recipientExists)
                {
                    throw new ArgumentException($"Người nhận với ID '{dto.RecipientUserId}' không tồn tại trong hệ thống.", nameof(dto.RecipientUserId));
                }
            }
            else // Loại thông báo không hợp lệ
            {
                throw new ArgumentException($"Loại thông báo không hợp lệ: '{dto.Type}'. Các loại hợp lệ: 'Chung', 'Đơn', 'Emergency'.", nameof(dto.Type));
            }

            // SentDate: Frontend sẽ gửi thời gian thực.
            // Nếu SentDate từ FE là null, Backend sẽ gán thời gian hiện tại.
            DateTime finalSentDate = dto.SentDate ?? DateTime.UtcNow;


            // === KẾT THÚC VALIDATION NGHIỆP VỤ ===


            var entity = new Notification
            {
                NotificationId = "NOTIF_" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper(), // Tạo ID duy nhất với tiền tố
                RecipientUserId = dto.RecipientUserId,
                Message = dto.Message,
                Type = dto.Type,
                SentDate = finalSentDate,
                IsRead = false // Luôn mặc định là false khi tạo mới thông báo
            };

            _context.Notifications.Add(entity);
            await _context.SaveChangesAsync();

            // Ánh xạ entity đã tạo thành NotificationDto để trả về
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

            // Ánh xạ các trường có thể cập nhật
            entity.Message = notificationDto.Message;
            entity.Type = notificationDto.Type;
            entity.SentDate = notificationDto.SentDate; // Cập nhật SentDate nếu FE gửi
            entity.IsRead = notificationDto.IsRead; // Cập nhật IsRead (thường là trường này)

            _context.Notifications.Update(entity);
            await _context.SaveChangesAsync();

            return notificationDto; // Trả về DTO đã gửi (hoặc DTO của entity sau khi load lại)
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
                .OrderByDescending(n => n.SentDate ?? DateTime.MinValue) // Sắp xếp an toàn cho nullable DateTime
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