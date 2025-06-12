// File: BloodDonation_System.Service.Implementation.DonationHistoryService.cs
using BloodDonation_System.Data;
using BloodDonation_System.Model.DTO.Blood;
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
            // 1. Tìm bản ghi DonationHistory hiện có
            var donationHistory = await _context.DonationHistories
                .FirstOrDefaultAsync(dh => dh.DonationId == donationId);

            if (donationHistory == null)
            {
                return null; // Không tìm thấy lịch sử hiến máu để cập nhật
            }

            // 2. Lưu trữ trạng thái cũ TRƯỚC KHI cập nhật để kiểm tra sau này
            var oldStatus = donationHistory.Status;

            
           
            // Cập nhật các thuộc tính non-nullable (hoặc luôn có giá trị)
            // (Đảm bảo các thuộc tính này luôn được DTO cung cấp giá trị hợp lệ)
            donationHistory.DonorUserId = dto.DonorUserId;
            donationHistory.DonationDate = dto.DonationDate;
            donationHistory.BloodTypeId = dto.BloodTypeId;
            donationHistory.ComponentId = dto.ComponentId;

            // Cập nhật các thuộc tính nullable, sử dụng ?? để giữ giá trị cũ nếu DTO cung cấp null
            donationHistory.QuantityMl = dto.QuantityMl ?? donationHistory.QuantityMl;
            donationHistory.EligibilityStatus = dto.EligibilityStatus ?? donationHistory.EligibilityStatus;
            donationHistory.ReasonIneligible = dto.ReasonIneligible ?? donationHistory.ReasonIneligible;
            donationHistory.TestingResults = dto.TestingResults ?? donationHistory.TestingResults;
            donationHistory.StaffUserId = dto.StaffUserId ?? donationHistory.StaffUserId;
            donationHistory.Status = dto.Status ?? donationHistory.Status; // Cập nhật trạng thái mới

            // Xử lý EmergencyId: Nếu DTO cung cấp null hoặc chuỗi rỗng, gán null.
            // Nếu có giá trị, kiểm tra sự tồn tại trong EmergencyRequests để đảm bảo tính hợp lệ.
            if (string.IsNullOrEmpty(dto.EmergencyId)) // Bao gồm cả null và chuỗi rỗng
            {
                donationHistory.EmergencyId = null; // Đặt thành NULL trong entity
            }
            else
            {
                // Kiểm tra sự tồn tại của EmergencyId nếu nó được cung cấp
                var emergencyExists = await _context.EmergencyRequests
                                                    .AnyAsync(er => er.EmergencyId == dto.EmergencyId);
                if (!emergencyExists)
                {
                    // Ném lỗi để báo cho người dùng rằng EmergencyId không hợp lệ
                    throw new ArgumentException($"The provided Emergency ID '{dto.EmergencyId}' does not exist.");
                }
                else
                {
                    donationHistory.EmergencyId = dto.EmergencyId; // Gán ID hợp lệ
                }
            }

            donationHistory.Descriptions = dto.Descriptions ?? donationHistory.Descriptions;

            // 4. Lưu thay đổi vào cơ sở dữ liệu
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.InnerException as Microsoft.Data.SqlClient.SqlException;
                if (sqlException != null)
                {
                    if (sqlException.Number == 547) // Foreign Key Violation hoặc NOT NULL constraint violation
                    {
                        Console.Error.WriteLine($"[DB_ERROR] Foreign Key/NULL Constraint Violation (547). " +
                                                $"EmergencyId: '{donationHistory.EmergencyId ?? "NULL"}' " +
                                                $"DonationRequestId: '{donationHistory.DonationRequestId ?? "NULL"}'. " +
                                                $"SQL Error: {sqlException.Message}");
                        // Thêm thông báo rõ ràng hơn về nguyên nhân
                        throw new InvalidOperationException("A foreign key or NOT NULL constraint was violated. " +
                                                            "This usually means the associated Emergency Request ID or Donation Request ID does not exist, " +
                                                            "OR you are trying to save NULL to a column that does not allow NULL in the database. " +
                                                            "Please ensure your database schema is up-to-date with your entity model (e.g., columns are nullable if intended).", ex);
                    }
                }
                Console.Error.WriteLine($"[DB_ERROR] An error occurred during database update: {ex.Message}");
                throw; // Ném lại ngoại lệ gốc
            }
            catch (ArgumentException ex) // Bắt lỗi ArgumentException mà chúng ta chủ động ném ra
            {
                Console.Error.WriteLine($"[VALIDATION_ERROR] {ex.Message}");
                throw;
            }
            catch (Exception ex) // Bắt các lỗi chung khác
            {
                Console.Error.WriteLine($"[GENERAL_ERROR] An unexpected error occurred: {ex.Message}");
                throw;
            }

            // 5. Lấy lại bản ghi sau khi cập nhật với các mối quan hệ cần thiết cho DTO trả về
            // Việc này cần thiết để có các đối tượng navigation properties (như DonorUser, BloodType)
            // đã được tải đầy đủ, phục vụ cho việc tạo DonationHistoryDetailDto.
            var updatedEntity = await _context.DonationHistories
                .Include(dh => dh.DonorUser).ThenInclude(u => u.UserProfile)
                .Include(dh => dh.BloodType)
                .Include(dh => dh.Component)
                .Include(dh => dh.StaffUser).ThenInclude(u => u.UserProfile)
                .FirstOrDefaultAsync(dh => dh.DonationId == donationId);

            if (updatedEntity == null)
            {
                // Điều này có thể xảy ra nếu bản ghi bị xóa ngay sau khi cập nhật nhưng trước khi được đọc lại,
                // mặc dù rất hiếm trong một luồng đồng bộ.
                return null;
            }

            // 6. LOGIC TẠO BLOODUNIT KHI TRẠNG THÁI CHUYỂN SANG "Complete"
            // (Chỉ chạy khi trạng thái thay đổi từ khác "Complete" sang "Complete")
            if (oldStatus != "Complete" && updatedEntity.Status == "Complete")
            {
                DateOnly expirationDate;
                int componentId = updatedEntity.ComponentId;

                switch (componentId)
                {
                    case 1: expirationDate = DateOnly.FromDateTime(updatedEntity.DonationDate.AddDays(42)); break; // Hồng cầu
                    case 2: expirationDate = DateOnly.FromDateTime(updatedEntity.DonationDate.AddYears(1)); break;  // Huyết tương
                    case 3: expirationDate = DateOnly.FromDateTime(updatedEntity.DonationDate.AddDays(5)); break;   // Tiểu cầu
                    case 4: expirationDate = DateOnly.FromDateTime(updatedEntity.DonationDate.AddDays(35)); break;  // Toàn phần
                    default:
                        expirationDate = DateOnly.FromDateTime(updatedEntity.DonationDate.AddDays(30)); // Mặc định
                        Console.WriteLine($"[WARNING] Unknown ComponentId {componentId} for DonationId {updatedEntity.DonationId}. Using default expiration date (30 days).");
                        break;
                }

                string assignedStorageLocation;
                switch (componentId)
                {
                    case 1: assignedStorageLocation = "COLD_STORAGE_A"; break;
                    case 2: assignedStorageLocation = "FREEZER_ZONE_P"; break;
                    case 3: assignedStorageLocation = "AGITATOR_ROOM_T"; break;
                    case 4: assignedStorageLocation = "REFRIGERATED_CABINET_W"; break;
                    default: assignedStorageLocation = "GENERAL_STORAGE_UNKNOWN"; break;
                }

                var newBloodUnit = new BloodUnit
                {
                    // Tạo UnitId duy nhất
                    UnitId = "BUITS_" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper(),
                    DonationId = updatedEntity.DonationId, // Sử dụng updatedEntity.DonationId
                    BloodTypeId = updatedEntity.BloodTypeId,
                    ComponentId = updatedEntity.ComponentId,
                    VolumeMl = updatedEntity.QuantityMl ?? 0, // QuantityMl là int? trong entity, VolumeMl là int trong BloodUnit entity
                    CollectionDate = DateOnly.FromDateTime(updatedEntity.DonationDate),
                    ExpirationDate = expirationDate,
                    StorageLocation = assignedStorageLocation,
                    TestResults = updatedEntity.TestingResults,
                    Status = "Available", // Trạng thái mặc định cho đơn vị máu mới tạo
                    DiscardReason = ""
                };

                // THÊM BLOODUNIT MỚI VÀO CONTEXT
                _context.BloodUnits.Add(newBloodUnit);

                // Lưu ý: Không cần await _context.SaveChangesAsync() riêng ở đây.
                // Việc lưu sẽ diễn ra cùng với DonationHistory trong khối try-catch lớn hơn.
                Console.WriteLine($"[INFO] BloodUnit marked for creation for DonationId: {updatedEntity.DonationId} with UnitId: {newBloodUnit.UnitId}.");
            }

            // 7. Chuyển đổi updatedEntity sang DonationHistoryDetailDto và trả về
            // Đảm bảo tất cả các thuộc tính DTO được ánh xạ, sử dụng toán tử null-conditional ?. để an toàn
            return new DonationHistoryDetailDto
            {
                DonationId = updatedEntity.DonationId,
                DonationRequestId = updatedEntity.DonationRequestId,
                DonorUserId = updatedEntity.DonorUserId,
                BloodTypeId = updatedEntity.BloodTypeId,
                ComponentId = updatedEntity.ComponentId,
                DonationDate = updatedEntity.DonationDate,
                QuantityMl = (int)updatedEntity.QuantityMl, // Giữ nguyên int? nếu DTO cho phép
                EligibilityStatus = updatedEntity.EligibilityStatus,
                ReasonIneligible = updatedEntity.ReasonIneligible,
                TestingResults = updatedEntity.TestingResults,
                StaffUserId = updatedEntity.StaffUserId,
                Status = updatedEntity.Status,
                EmergencyId = updatedEntity.EmergencyId,
                Descriptions = updatedEntity.Descriptions,

                // Các thuộc tính từ các bảng liên quan (sử dụng null-conditional operator)
                DonorUserName = updatedEntity.DonorUser?.UserProfile?.FullName,
                BloodTypeName = updatedEntity.BloodType?.TypeName,
                ComponentName = updatedEntity.Component?.ComponentName,
              
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