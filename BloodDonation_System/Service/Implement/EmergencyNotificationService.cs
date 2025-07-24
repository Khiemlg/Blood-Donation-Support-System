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
        private readonly IEmailService _emailService;

        public EmergencyNotificationService(DButils context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        public async Task<string> RespondToEmergencyNotificationAsync(string userId, EmergencyResponseDTO dto)
        {
            var notification = await _context.EmergencyNotifications
                .FirstOrDefaultAsync(n => n.NotificationId == dto.NotificationId && n.RecipientUserId == userId);

            if (notification == null)
                return "❌ Không tìm thấy thông báo hoặc không thuộc về bạn.";

            if (notification.ResponseStatus == "Interested" || notification.ResponseStatus == "Declined")
                return "⚠️ Bạn đã phản hồi rồi. Không thể sửa phản hồi.";

            if (dto.ResponseStatus != "Interested" && dto.ResponseStatus != "Declined")
                return "❌ Trạng thái phản hồi không hợp lệ.";

            notification.ResponseStatus = dto.ResponseStatus;
            notification.IsRead = true; 
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
                    EmergencyId = request.EmergencyId,
                    RecipientUserId = member.UserId,
                    SentDate = DateTime.Now,
                    DeliveryMethod = "App Notification", 
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

        public async Task<EmergencyNotificationDto> CreateAsyncbyStaff(EmergencyNotificationInputDto dto)
        {
            var entity = new EmergencyNotification
            {
                NotificationId = "NO_EN_" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper(),
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

            var recipient = await _context.Users
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.UserId == dto.RecipientUserId && u.Email != null);

            var emergency = await _context.EmergencyRequests
                .Include(e => e.BloodType)
                .FirstOrDefaultAsync(e => e.EmergencyId == dto.EmergencyId);

            if (recipient != null && emergency != null)
            {
                string fullName = recipient.UserProfile?.FullName ?? "bạn";
                string bloodTypeName = emergency.BloodType?.TypeName ?? "Không rõ";
                string subject = "🩸 Yêu cầu hỗ trợ hiến máu khẩn cấp dành cho bạn";

                string emailBody = $@"
<p>Xin chào {fullName},</p>
<p>Bạn vừa nhận được một <strong>thông báo khẩn cấp</strong> từ hệ thống.</p>
<ul>
    <li><strong>Nhóm máu:</strong> {bloodTypeName}</li>
    <li><strong>Số lượng cần:</strong> {emergency.QuantityNeededMl} ml</li>
    <li><strong>Hạn chót:</strong> {emergency.DueDate:dd/MM/yyyy}</li>
    <li><strong>Ưu tiên:</strong> {emergency.Priority}</li>
    <li><strong>Mô tả:</strong> {dto.Message}</li>
</ul>
<p>🙏 Nếu bạn sẵn sàng hỗ trợ, vui lòng phản hồi sớm hoặc đến trung tâm hiến máu gần nhất.</p>
<p>Trân trọng,<br/>Hệ thống Hiến Máu Tình Nguyện</p>";

                await _emailService.SendEmailAsync(recipient.Email, subject, emailBody);
            }
            // Nếu priority là MEDIUM hoặc HIGH => tạo thông báo cho những người phù hợp
            /*  if (dto.Message.Contains("Medium", StringComparison.OrdinalIgnoreCase) ||
         dto.Message.Contains("High", StringComparison.OrdinalIgnoreCase))
              {
                  var now = DateTime.UtcNow;

                  var eligibleUsers = await _context.Users
                      .Include(u => u.UserProfile)
                      .Where(u =>
                          u.BloodTypeId == emergency.BloodTypeId &&
                          u.UserId != dto.RecipientUserId &&
                          u.Status == "Available" &&
                          u.UserProfile != null
                      )
                      .ToListAsync();

                  foreach (var user in eligibleUsers)
                  {
                      var noti = new EmergencyNotification
                      {
                          NotificationId = "NO_EN_" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper(),
                          EmergencyId = dto.EmergencyId,
                          RecipientUserId = user.UserId,
                          SentDate = DateTime.UtcNow,
                          DeliveryMethod = "InApp",
                          IsRead = false,
                          Message = $"[Khẩn cấp] Cần {emergency.QuantityNeededMl}ml máu nhóm {bloodType}. Chi tiết: {dto.Message}",
                          ResponseStatus = "Pending"
                      };
                      _context.EmergencyNotifications.Add(noti);
                  }

                  await _context.SaveChangesAsync();
              }*/

            // Nếu priority là MEDIUM hoặc HIGH => tạo thông báo cho những người phù hợp
            if (dto.Message.Contains("Medium", StringComparison.OrdinalIgnoreCase) ||
                dto.Message.Contains("High", StringComparison.OrdinalIgnoreCase))
            {
                var now = DateTime.UtcNow;
                var thresholdDate = now.AddDays(-90);

                var eligibleUsers = await _context.Users
                    .Include(u => u.UserProfile)
                    .Where(u =>
                        u.UserProfile.BloodTypeId == emergency.BloodTypeId &&
                        u.UserId != dto.RecipientUserId &&
                        u.UserProfile != null &&
                        (
                            !_context.DonationHistory.Any(bd => bd.DonorUserId == u.UserId) || // chưa từng hiến
                            _context.DonationHistory
                                .Where(bd => bd.DonorUserId == u.UserId)
                                .OrderByDescending(bd => bd.DonationDate)
                                .Select(bd => bd.DonationDate)
                                .FirstOrDefault() <= thresholdDate // lần hiến máu gần nhất >= 90 ngày
                        )
                    )
                    .ToListAsync();

                foreach (var user in eligibleUsers)
                {
                    var noti = new EmergencyNotification
                    {
                        NotificationId = "NO_EN_" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper(),
                        EmergencyId = dto.EmergencyId,
                        RecipientUserId = user.UserId,
                        SentDate = DateTime.UtcNow,
                        DeliveryMethod = "InApp",
                        IsRead = false,
                        Message = dto.Message,
                        ResponseStatus = "Pending"
                    };

                    _context.EmergencyNotifications.Add(noti);
                }

                await _context.SaveChangesAsync();
            }

            return new EmergencyNotificationDto
            {
                NotificationId = entity.NotificationId,
                EmergencyId = entity.EmergencyId,
                RecipientUserId = entity.RecipientUserId,
                SentDate = entity.SentDate,
                DeliveryMethod = entity.DeliveryMethod,
                IsRead = entity.IsRead,
                Message = entity.Message,
                ResponseStatus = entity.ResponseStatus
            };
        }

        public async Task<IEnumerable<EmergencyNotificationDto>> GetByUserIdAsync(string userId)
        {
            return await _context.EmergencyNotifications
                .Where(n => n.RecipientUserId == userId)
                .OrderByDescending(n => n.SentDate) 
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
