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
    public class EmergencyRequestService : IEmergencyRequestService
    {
        private readonly DButils _context;
        
        private readonly IEmergencyNotificationService _emergencyNotificationService;


        public EmergencyRequestService(
     DButils context,
     IEmergencyNotificationService emergencyNotificationService)
        {
            _context = context;
            _emergencyNotificationService = emergencyNotificationService;
        }

        // code để tạo yêu cầu máu khẩn cấp từ Staff 8/6/-15h code by khiem
        public async Task<(bool Success, string Message)> CreateEmergencyRequestAsync(EmergencyRequestCreateDto dto, string staffUserId)
        {
            var bloodTypeName = await _context.BloodTypes
    .Where(bt => bt.BloodTypeId == dto.BloodTypeId)
    .Select(bt => bt.TypeName)
    .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(bloodTypeName))
                return (false, "Không tìm thấy nhóm máu phù hợp.");

            var emergency = new EmergencyRequest
            {
                EmergencyId = "EMER_" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper(),
                RequesterUserId = staffUserId,
                BloodTypeId = dto.BloodTypeId,
                ComponentId = dto.ComponentId,
                QuantityNeededMl = dto.QuantityNeededMl,
                Priority = dto.Priority,
                DueDate = dto.DueDate,
                Description = dto.Description,
                CreationDate = DateTime.Now,
                Status = "Pending"
            };

            await _context.EmergencyRequests.AddAsync(emergency);
            await _context.SaveChangesAsync();

            var notification = new EmergencyNotification
            {
                NotificationId = "NO_EN_" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper(),
                EmergencyId = emergency.EmergencyId,
                RecipientUserId = "ALL",
                SentDate = DateTime.Now,
                DeliveryMethod = "System",
                IsRead = false,
                ResponseStatus = null,
                Message = $"📢 Yêu cầu khẩn cấp: {dto.QuantityNeededMl}ml máu nhóm {bloodTypeName}, ưu tiên {dto.Priority}. Mô tả: {dto.Description}"
            };

            _context.EmergencyNotifications.Add(notification);
            await _context.SaveChangesAsync();

            return (true, "✅ Yêu cầu khẩn cấp đã được tạo và thông báo hệ thống đã ghi nhận.");
        }

        // -------------------------------------------

        public async Task<IEnumerable<EmergencyRequestDto>> GetAllAsync()
        {
            return await _context.EmergencyRequests
                .Select(er => new EmergencyRequestDto
                {
                    EmergencyId = er.EmergencyId,
                    RequesterUserId = er.RequesterUserId,
                    BloodTypeId = er.BloodTypeId,
                    ComponentId = er.ComponentId,
                    QuantityNeededMl = er.QuantityNeededMl,
                    Priority = er.Priority,
                    DueDate = er.DueDate,
                    CreationDate = er.CreationDate,
                    FulfillmentDate = er.FulfillmentDate,
                    Description = er.Description,
                    Status = er.Status
                }).ToListAsync();
        }

        public async Task<EmergencyRequestDto?> GetByIdAsync(string emergencyId)
        {
            var er = await _context.EmergencyRequests.FindAsync(emergencyId);
            if (er == null) return null;

            return new EmergencyRequestDto
            {
                EmergencyId = er.EmergencyId,
                RequesterUserId = er.RequesterUserId,
                BloodTypeId = er.BloodTypeId,
                ComponentId = er.ComponentId,
                QuantityNeededMl = er.QuantityNeededMl,
                Priority = er.Priority,
                DueDate = er.DueDate,
                CreationDate = er.CreationDate,
                FulfillmentDate = er.FulfillmentDate,
                Description = er.Description,
                Status = er.Status
            };
        }

        public async Task<EmergencyRequestDto> CreateAsync(EmergencyRequestDto dto)
        {
            var entity = new EmergencyRequest
            {
                EmergencyId = Guid.NewGuid().ToString(),
                RequesterUserId = dto.RequesterUserId,
                BloodTypeId = dto.BloodTypeId,
                ComponentId = dto.ComponentId,
                QuantityNeededMl = dto.QuantityNeededMl,
                Priority = dto.Priority,
                DueDate = dto.DueDate,
                CreationDate = dto.CreationDate ?? DateTime.UtcNow,
                FulfillmentDate = dto.FulfillmentDate,
                Description = dto.Description,
                Status = dto.Status
            };

            _context.EmergencyRequests.Add(entity);
            await _context.SaveChangesAsync();

            dto.EmergencyId = entity.EmergencyId;
            return dto;
        }

        public async Task<EmergencyRequestDto?> UpdateAsync(string emergencyId, EmergencyRequestDto dto)
        {
            var entity = await _context.EmergencyRequests.FindAsync(emergencyId);
            if (entity == null) return null;

            entity.RequesterUserId = dto.RequesterUserId;
            entity.BloodTypeId = dto.BloodTypeId;
            entity.ComponentId = dto.ComponentId;
            entity.QuantityNeededMl = dto.QuantityNeededMl;
            entity.Priority = dto.Priority;
            entity.DueDate = dto.DueDate;
            entity.CreationDate = dto.CreationDate;
            entity.FulfillmentDate = dto.FulfillmentDate;
            entity.Description = dto.Description;
            entity.Status = dto.Status;

            _context.EmergencyRequests.Update(entity);
            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<bool> DeleteAsync(string emergencyId)
        {
            var entity = await _context.EmergencyRequests.FindAsync(emergencyId);
            if (entity == null) return false;

            _context.EmergencyRequests.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        // các hàm thêm bơi khiêm -------------------------------------- 25/06/2024
        // Lấy tất cả yêu cầu máu khẩn cấp, có thể lọc theo trạng thái
        public async Task<IEnumerable<EmergencyRequestDto>> GetAllEmergencyRequestsAsync(string? status)
        {
            var query = _context.EmergencyRequests.AsQueryable();
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(x => x.Status == status);
            }
            return await query
                .Select(er => new EmergencyRequestDto
                {
                    EmergencyId = er.EmergencyId,
                    RequesterUserId = er.RequesterUserId,
                    BloodTypeId = er.BloodTypeId,
                    ComponentId = er.ComponentId,
                    QuantityNeededMl = er.QuantityNeededMl,
                    Priority = er.Priority,
                    DueDate = er.DueDate,
                    CreationDate = er.CreationDate,
                    FulfillmentDate = er.FulfillmentDate,
                    Description = er.Description,
                    Status = er.Status
                }).ToListAsync();
        }

        // Lấy chi tiết một yêu cầu máu khẩn cấp theo ID
        public async Task<EmergencyRequestDto?> GetEmergencyRequestByIdAsync(string emergencyId)
        {
            var er = await _context.EmergencyRequests.FindAsync(emergencyId);
            if (er == null) return null;
            return new EmergencyRequestDto
            {
                EmergencyId = er.EmergencyId,
                RequesterUserId = er.RequesterUserId,
                BloodTypeId = er.BloodTypeId,
                ComponentId = er.ComponentId,
                QuantityNeededMl = er.QuantityNeededMl,
                Priority = er.Priority,
                DueDate = er.DueDate,
                CreationDate = er.CreationDate,
                FulfillmentDate = er.FulfillmentDate,
                Description = er.Description,
                Status = er.Status
            };
        }

        // Cập nhật trạng thái yêu cầu máu khẩn cấp
        public async Task<(bool Success, string Message)> UpdateEmergencyRequestStatusAsync(string emergencyId, string status)
        {
            var entity = await _context.EmergencyRequests.FindAsync(emergencyId);
            if (entity == null) return (false, "Request not found");
            entity.Status = status;
            _context.EmergencyRequests.Update(entity);
            await _context.SaveChangesAsync();
            return (true, "Status updated successfully");
        }

        // Gửi thông báo khẩn cấp tới các donor phù hợp
        public async Task<(bool Success, string Message)> NotifyDonorsForEmergencyAsync(string emergencyId)
        {
            var emergency = await _context.EmergencyRequests.FindAsync(emergencyId);
            if (emergency == null) return (false, "Emergency request not found");
            await _emergencyNotificationService.NotifyMatchingMembersAsync(emergency);
            return (true, "Notifications sent to matching donors");
        }

        // Lấy danh sách yêu cầu máu khẩn cấp của một user
        public async Task<IEnumerable<EmergencyRequestDto>> GetEmergencyRequestsByUserAsync(string userId)
        {
            return await _context.EmergencyRequests
                .Where(x => x.RequesterUserId == userId)
                .Select(er => new EmergencyRequestDto
                {
                    EmergencyId = er.EmergencyId,
                    RequesterUserId = er.RequesterUserId,
                    BloodTypeId = er.BloodTypeId,
                    ComponentId = er.ComponentId,
                    QuantityNeededMl = er.QuantityNeededMl,
                    Priority = er.Priority,
                    DueDate = er.DueDate,
                    CreationDate = er.CreationDate,
                    FulfillmentDate = er.FulfillmentDate,
                    Description = er.Description,
                    Status = er.Status
                }).ToListAsync();
        }

        // Hủy yêu cầu máu khẩn cấp
        public async Task<(bool Success, string Message)> CancelEmergencyRequestAsync(string emergencyId)
        {
            var entity = await _context.EmergencyRequests.FindAsync(emergencyId);
            if (entity == null) return (false, "Request not found");
            entity.Status = "Cancelled";
            _context.EmergencyRequests.Update(entity);
            await _context.SaveChangesAsync();
            return (true, "Request cancelled successfully");
        }


    }
}
