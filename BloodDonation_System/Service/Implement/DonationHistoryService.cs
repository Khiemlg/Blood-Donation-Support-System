// File: BloodDonation_System.Service.Implementation.DonationHistoryService.cs
using BloodDonation_System.Data;
using BloodDonation_System.Model.DTO.Donation;
using BloodDonation_System.Model.Enties;
using BloodDonation_System.Service.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BloodDonation_System.Service.Implement
{
    public class DonationHistoryService : IDonationHistoryService
    {
        private readonly DButils _context;

        public DonationHistoryService(DButils context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DonationHistoryDetailDto>> GetAllAsync()
        {
            return await _context.DonationHistories
                .Include(dh => dh.DonorUser)
                    .ThenInclude(u => u.UserProfile)
                .Include(dh => dh.BloodType)
                .Include(dh => dh.Component) // Map to BloodComponent if named differently in DBContext
                .Select(dh => new DonationHistoryDetailDto
                {
                    DonationId = dh.DonationId,
                    DonorUserId = dh.DonorUserId,
                    DonorUserName = dh.DonorUser.UserProfile.FullName,
                    DonationDate = dh.DonationDate,
                    BloodTypeId = dh.BloodTypeId,
                    BloodTypeName = dh.BloodType.TypeName,
                    ComponentId = dh.ComponentId,
                    ComponentName = dh.Component.ComponentName, // Ensure this matches your BloodComponent property
                    QuantityMl = (int)dh.QuantityMl,
                    EligibilityStatus = dh.EligibilityStatus,
                    ReasonIneligible = dh.ReasonIneligible,
                    TestingResults = dh.TestingResults,
                    StaffUserId = dh.StaffUserId,
                    Status = dh.Status,
                    EmergencyId = dh.EmergencyId,
                    Descriptions = dh.Descriptions,
                    DonationRequestId = dh.DonationRequestId
                })
                .ToListAsync();
        }

        public async Task<DonationHistoryDetailDto?> GetByIdAsync(string donationId)
        {
            var dh = await _context.DonationHistories
                .Include(h => h.DonorUser)
                    .ThenInclude(u => u.UserProfile)
                .Include(h => h.BloodType)
                .Include(h => h.Component)
                .FirstOrDefaultAsync(h => h.DonationId == donationId);

            if (dh == null) return null;

            return new DonationHistoryDetailDto
            {
                DonationId = dh.DonationId,
                DonorUserId = dh.DonorUserId,
                DonorUserName = dh.DonorUser?.UserProfile?.FullName,
                DonationDate = dh.DonationDate,
                BloodTypeId = dh.BloodTypeId,
                BloodTypeName = dh.BloodType?.TypeName,
                ComponentId = dh.ComponentId,
                ComponentName = dh.Component?.ComponentName,
                QuantityMl = (int)dh.QuantityMl,
                EligibilityStatus = dh.EligibilityStatus,
                ReasonIneligible = dh.ReasonIneligible,
                TestingResults = dh.TestingResults,
                StaffUserId = dh.StaffUserId,
                Status = dh.Status,
                EmergencyId = dh.EmergencyId,
                Descriptions = dh.Descriptions,
                DonationRequestId = dh.DonationRequestId
            };
        }

        public async Task<DonationHistoryDto> CreateAsync(DonationHistoryDto dto)
        {
            var entity = new DonationHistory
            {
                DonationId = "BUnits_" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper(),
                DonorUserId = dto.DonorUserId,
                DonationDate = dto.DonationDate,
                BloodTypeId = dto.BloodTypeId,
                ComponentId = dto.ComponentId,
                QuantityMl = dto.QuantityMl,
                EligibilityStatus = dto.EligibilityStatus,
                ReasonIneligible = dto.ReasonIneligible,
                TestingResults = dto.TestingResults,
                StaffUserId = dto.StaffUserId,
                Status = dto.Status,
                EmergencyId = dto.EmergencyId,
                Descriptions = dto.Descriptions,
                DonationRequestId = dto.DonationRequestId
            };

            _context.DonationHistories.Add(entity);
            await _context.SaveChangesAsync();

            dto.DonationId = entity.DonationId;
            return dto;
        }

     // Trong BloodDonation_System.Service.Implement.DonationHistoryService.cs

public async Task<DonationHistoryDetailDto?> UpdateAsync(string donationId, DonationHistoryUpdateDto dto)
{
    var entity = await _context.DonationHistories
                               .FirstOrDefaultAsync(dh => dh.DonationId == donationId);

    if (entity == null) return null;

    // Cập nhật các trường khác
    entity.DonorUserId = dto.DonorUserId;
    entity.DonationDate = dto.DonationDate;
    entity.BloodTypeId = dto.BloodTypeId;
    entity.ComponentId = dto.ComponentId;
    entity.QuantityMl = dto.QuantityMl;
    entity.EligibilityStatus = dto.EligibilityStatus;
    entity.ReasonIneligible = dto.ReasonIneligible;
    entity.TestingResults = dto.TestingResults;
    entity.StaffUserId = dto.StaffUserId;
    entity.Status = dto.Status;
    // entity.DonationRequestId = dto.DonationRequestId; // Không nên cập nhật DonationRequestId nếu nó được set bởi hệ thống
                                                      // hoặc chỉ khi có logic rõ ràng để thay đổi nó.
                                                      // Trong DTO của bạn, nó là internal set, nên controller không gửi lên.
    entity.Descriptions = dto.Descriptions;

    
    

    _context.DonationHistories.Update(entity);
    await _context.SaveChangesAsync(); // Lỗi sẽ được giải quyết nếu ID hợp lệ hoặc là null hợp lệ

    // Tải lại các navigation properties để tạo Response DTO đầy đủ (như đã làm trước đó)
    var updatedAndLoadedHistory = await _context.DonationHistories
        .Include(dh => dh.DonationRequest)
        .Include(dh => dh.DonorUser)
            .ThenInclude(u => u.UserProfile)
        .Include(dh => dh.BloodType)
        .Include(dh => dh.Component)
        .Include(dh => dh.StaffUser)
            .ThenInclude(u => u.UserProfile)
        .Include(dh => dh.Emergency) // Đảm bảo Emergency (EmergencyRequest) được tải
        .FirstOrDefaultAsync(dh => dh.DonationId == donationId);

    // Ánh xạ entity đã cập nhật thành DonationHistoryDto để trả về
    // Đảm bảo rằng DonationHistoryDto có đủ các trường để ánh xạ từ entity này.
    return new DonationHistoryDetailDto
    {
        DonationId = updatedAndLoadedHistory.DonationId,
        DonationRequestId = updatedAndLoadedHistory.DonationRequestId,
        DonorUserId = updatedAndLoadedHistory.DonorUserId,
        BloodTypeId = updatedAndLoadedHistory.BloodTypeId,
        ComponentId = updatedAndLoadedHistory.ComponentId,
        DonationDate = updatedAndLoadedHistory.DonationDate,
        QuantityMl = (int)updatedAndLoadedHistory.QuantityMl,
        EligibilityStatus = updatedAndLoadedHistory.EligibilityStatus,
        ReasonIneligible = updatedAndLoadedHistory.ReasonIneligible,
        TestingResults = updatedAndLoadedHistory.TestingResults,
        StaffUserId = updatedAndLoadedHistory.StaffUserId,
        Status = updatedAndLoadedHistory.Status,
        EmergencyId = updatedAndLoadedHistory.EmergencyId, // Đảm bảo EmergencyId được gán
        Descriptions = updatedAndLoadedHistory.Descriptions,
        DonorUserName = updatedAndLoadedHistory.DonorUser?.UserProfile?.FullName,
        BloodTypeName = updatedAndLoadedHistory.BloodType?.TypeName,
        ComponentName = updatedAndLoadedHistory.Component?.ComponentName
      //  StaffUserName = updatedAndLoadedHistory.StaffUser?.UserProfile?.FullName
        // Nếu bạn có EmergencyRequestDetailDto, có thể ánh xạ ở đây:
        // EmergencyRequestDetail = updatedAndLoadedHistory.Emergency != null ? new EmergencyRequestDetailDto { /* map properties */ } : null
    };
}
        public async Task<bool> DeleteAsync(string donationId)
        {
            var entity = await _context.DonationHistories.FindAsync(donationId);
            if (entity == null) return false;

            _context.DonationHistories.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<DonationHistoryDetailDto?> GetDonationHistoryByRequestIdAsync(string requestId)
        {
            var dh = await _context.DonationHistories
                                   .Include(h => h.DonorUser)
                                       .ThenInclude(u => u.UserProfile)
                                   .Include(h => h.BloodType)
                                   .Include(h => h.Component)
                                   .FirstOrDefaultAsync(h => h.DonationRequestId.Equals(requestId));

            if (dh == null)
            {
                return null;
            }

            return new DonationHistoryDetailDto
            {
                DonationId = dh.DonationId,
                DonorUserId = dh.DonorUserId,
                DonorUserName = dh.DonorUser?.UserProfile?.FullName,
                DonationDate = dh.DonationDate,
                BloodTypeId = dh.BloodTypeId,
                BloodTypeName = dh.BloodType?.TypeName,
                ComponentId = dh.ComponentId,
                ComponentName = dh.Component?.ComponentName,
                QuantityMl = (int)dh.QuantityMl,
                EligibilityStatus = dh.EligibilityStatus,
                ReasonIneligible = dh.ReasonIneligible,
                TestingResults = dh.TestingResults,
                StaffUserId = dh.StaffUserId,
                Status = dh.Status,
                EmergencyId = dh.EmergencyId,
                Descriptions = dh.Descriptions,
                DonationRequestId = dh.DonationRequestId
            };
        }

        public async Task<IEnumerable<DonationHistoryDetailDto>> GetHistoryByDonorUserIdAsync(string userId)
        {
            return await _context.DonationHistories
                .Where(dh => dh.DonorUserId == userId) // Lọc theo DonorUserId
                .Include(dh => dh.DonationRequest)
                .Include(dh => dh.DonorUser)
                    .ThenInclude(u => u.UserProfile)
                .Include(dh => dh.BloodType)
                .Include(dh => dh.Component)
                .Include(dh => dh.StaffUser)
                    .ThenInclude(u => u.UserProfile)
                .Select(dh => new DonationHistoryDetailDto
                {
                    DonationId = dh.DonationId,
                    DonationRequestId = dh.DonationRequestId,
                    DonorUserId = dh.DonorUserId,
                    BloodTypeId = dh.BloodTypeId,
                    ComponentId = dh.ComponentId,
                    DonationDate = dh.DonationDate,
                    QuantityMl = (int)dh.QuantityMl,
                    EligibilityStatus = dh.EligibilityStatus,
                    ReasonIneligible = dh.ReasonIneligible,
                    TestingResults = dh.TestingResults,
                    StaffUserId = dh.StaffUserId,
                    Status = dh.Status,
                    EmergencyId = dh.EmergencyId,
                    Descriptions = dh.Descriptions,
                    DonorUserName = dh.DonorUser.UserProfile != null ? dh.DonorUser.UserProfile.FullName : null,
                    BloodTypeName = dh.BloodType != null ? dh.BloodType.TypeName : null,
                    ComponentName = dh.Component != null ? dh.Component.ComponentName : null,
                  //  StaffUserName = dh.StaffUser.UserProfile != null ? dh.StaffUser.UserProfile.FullName : null
                })
                .ToListAsync();
        }
    }
}