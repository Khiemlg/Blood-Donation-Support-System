using BloodDonation_System.Data;
using BloodDonation_System.Model.DTO.Blood;
using BloodDonation_System.Model.Enties;
using BloodDonation_System.Service.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BloodDonation_System.Service.Implement
{
    public class BloodUnitService : IBloodUnitService
    {
        private readonly DButils _context;

        public BloodUnitService(DButils context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all blood units, mapped to the basic BloodUnitDto.
        /// </summary>
        /// <returns>A collection of BloodUnitDto.</returns>
        public async Task<IEnumerable<BloodUnitDto>> GetAllAsync()
        {
            return await _context.BloodUnits
                .Select(bu => new BloodUnitDto
                {
                    UnitId = bu.UnitId,
                    DonationId = bu.DonationId,
                    BloodTypeId = bu.BloodTypeId,
                    ComponentId = bu.ComponentId,
                    VolumeMl = bu.VolumeMl,
                    CollectionDate = bu.CollectionDate,
                    ExpirationDate = bu.ExpirationDate,
                    StorageLocation = bu.StorageLocation,
                    TestResults = bu.TestResults,
                    Status = bu.Status,
                    DiscardReason = bu.DiscardReason
                })
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a single blood unit by its ID, mapped to the basic BloodUnitDto.
        /// </summary>
        /// <param name="unitId">The ID of the blood unit.</param>
        /// <returns>A BloodUnitDto if found, otherwise null.</returns>
        public async Task<BloodUnitDto?> GetByIdAsync(string unitId)
        {
            var bu = await _context.BloodUnits.FindAsync(unitId);
            if (bu == null) return null;

            return new BloodUnitDto
            {
                UnitId = bu.UnitId,
                DonationId = bu.DonationId,
                BloodTypeId = bu.BloodTypeId,
                ComponentId = bu.ComponentId,
                VolumeMl = bu.VolumeMl,
                CollectionDate = bu.CollectionDate,
                ExpirationDate = bu.ExpirationDate,
                StorageLocation = bu.StorageLocation,
                TestResults = bu.TestResults,
                Status = bu.Status,
                DiscardReason = bu.DiscardReason
            };
        }

        /// <summary>
        /// Creates a new blood unit from the provided DTO.
        /// </summary>
        /// <param name="dto">The BloodUnitDto containing the data for the new unit.</param>
        /// <returns>The created BloodUnitDto with the generated UnitId.</returns>
        public async Task<BloodUnitDto> CreateAsync(BloodUnitDto dto)
        {
            // 1. Tính toán ExpirationDate dựa trên ComponentId
            DateOnly expirationDate;
            switch (dto.ComponentId)
            {
                case 1: // Hồng cầu (Red Blood Cells)
                    expirationDate = dto.CollectionDate.AddDays(42); // Khoảng 6 tuần
                    break;
                case 2: // Huyết tương (Plasma)
                    expirationDate = dto.CollectionDate.AddYears(1); // Thường là 1 năm
                    break;
                case 3: // Tiểu cầu (Platelets)
                    expirationDate = dto.CollectionDate.AddDays(5); // Rất ngắn
                    break;
                case 4: // Máu toàn phần (Whole Blood)
                    expirationDate = dto.CollectionDate.AddDays(35); // Khoảng 5 tuần
                    break;
                default:
                    // Giá trị mặc định nếu ComponentId không xác định hoặc không có quy tắc cụ thể
                    expirationDate = dto.CollectionDate.AddDays(30);
                    Console.WriteLine($"[WARNING] Unknown ComponentId {dto.ComponentId}. Using default expiration date (30 days).");
                    break;
            }

            // 2. Gán StorageLocation dựa trên ComponentId
            string assignedStorageLocation;
            switch (dto.ComponentId)
            {
                case 1: // Hồng cầu
                    assignedStorageLocation = "COLD_STORAGE_A";
                    break;
                case 2: // Huyết tương
                    assignedStorageLocation = "FREEZER_ZONE_P";
                    break;
                case 3: // Tiểu cầu
                    assignedStorageLocation = "AGITATOR_ROOM_T";
                    break;
                case 4: // Máu toàn phần
                    assignedStorageLocation = "REFRIGERATED_CABINET_W";
                    break;
                default:
                    assignedStorageLocation = "GENERAL_STORAGE_UNKNOWN";
                    Console.WriteLine($"[WARNING] Unknown ComponentId {dto.ComponentId}. Assigning to default storage: {assignedStorageLocation}.");
                    break;
            }

            // 3. Tạo BloodUnit entity
            var entity = new BloodUnit
            {
                // UnitId: Tạo ID duy nhất tại đây.
                // Nếu bạn muốn định dạng "Buits_" + GUID, hãy làm như sau:
                UnitId = "BUITS_" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper(),
                // Nếu bạn muốn Guid.NewGuid().ToString() đầy đủ, hãy dùng:
                // UnitId = Guid.NewGuid().ToString(),

                DonationId = dto.DonationId,
                BloodTypeId = dto.BloodTypeId,
                ComponentId = dto.ComponentId,
                VolumeMl = dto.VolumeMl,
                CollectionDate = dto.CollectionDate,
                ExpirationDate = expirationDate,     // <-- Đã tính toán
                StorageLocation = assignedStorageLocation, // <-- Đã xác định
                TestResults = dto.TestResults,
                Status = dto.Status, // Có thể mặc định là "Available" nếu bạn không muốn DTO cung cấp
                DiscardReason = dto.DiscardReason    // Thường là null khi tạo mới
            };

            // 4. Thêm vào DbContext và lưu vào database
            _context.BloodUnits.Add(entity);
            await _context.SaveChangesAsync();

            // 5. Cập nhật DTO với các giá trị đã được hệ thống gán/tạo
            dto.UnitId = entity.UnitId;
            dto.ExpirationDate = entity.ExpirationDate;
            dto.StorageLocation = entity.StorageLocation;

            return dto; // Trả về DTO đã được cập nhật
        }

        /// <summary>
        /// Updates an existing blood unit.
        /// </summary>
        /// <param name="unitId">The ID of the blood unit to update.</param>
        /// <param name="dto">The BloodUnitDto containing the updated data.</param>
        /// <returns>The updated BloodUnitDto if found, otherwise null.</returns>
        public async Task<BloodUnitDto?> UpdateAsync(string unitId, BloodUnitDto dto)
        {
            // Use FirstOrDefaultAsync with predicate as FindAsync doesn't support eager loading
            var entity = await _context.BloodUnits.FirstOrDefaultAsync(bu => bu.UnitId == unitId);
            if (entity == null) return null;

            entity.DonationId = dto.DonationId;
            entity.BloodTypeId = dto.BloodTypeId;
            entity.ComponentId = dto.ComponentId;
            entity.VolumeMl = dto.VolumeMl;
            entity.CollectionDate = dto.CollectionDate;
            entity.ExpirationDate = dto.ExpirationDate;
            entity.StorageLocation = dto.StorageLocation;
            entity.TestResults = dto.TestResults;
            entity.Status = dto.Status;
            entity.DiscardReason = dto.DiscardReason;

            // _context.BloodUnits.Update(entity); // Not necessary if entity is tracked and modified
            await _context.SaveChangesAsync();

            return dto; // Return the updated DTO
        }

        /// <summary>
        /// Deletes a blood unit by its ID.
        /// </summary>
        /// <param name="unitId">The ID of the blood unit to delete.</param>
        /// <returns>True if the unit was deleted, false if not found.</returns>
        public async Task<bool> DeleteAsync(string unitId)
        {
            var entity = await _context.BloodUnits.FindAsync(unitId);
            if (entity == null) return false;

            _context.BloodUnits.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        // --- New Methods for BloodUnitInventoryDto ---

        /// <summary>
        /// Retrieves all blood units with detailed inventory information (including BloodType and Component names).
        /// </summary>
        /// <returns>A collection of BloodUnitInventoryDto.</returns>
        public async Task<IEnumerable<BloodUnitInventoryDto>> GetInventoryUnitsAsync()
        {
            return await _context.BloodUnits
                .Include(bu => bu.BloodType)      // Eager load BloodType for its name
                .Include(bu => bu.Component)      // Eager load BloodComponent for its name
                .Select(bu => new BloodUnitInventoryDto // Project into BloodUnitInventoryDto
                {
                    UnitId = bu.UnitId,
                    DonationId = bu.DonationId,
                    BloodTypeId = bu.BloodTypeId,
                    BloodTypeName = bu.BloodType.TypeName,      // Get name from BloodType
                    ComponentId = bu.ComponentId,
                    ComponentName = bu.Component.ComponentName, // Get name from BloodComponent
                    VolumeMl = bu.VolumeMl,
                    CollectionDate = bu.CollectionDate,
                    ExpirationDate = bu.ExpirationDate,
                    StorageLocation = bu.StorageLocation,
                    TestResults = bu.TestResults,
                    Status = bu.Status,
                    DiscardReason = bu.DiscardReason
                })
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a single blood unit by its ID with detailed inventory information.
        /// </summary>
        /// <param name="unitId">The ID of the blood unit.</param>
        /// <returns>A BloodUnitInventoryDto if found, otherwise null.</returns>
        public async Task<BloodUnitInventoryDto?> GetInventoryUnitByIdAsync(string unitId)
        {
            var bu = await _context.BloodUnits
                .Include(b => b.BloodType)      // Eager load BloodType
                .Include(b => b.Component)      // Eager load BloodComponent
                .FirstOrDefaultAsync(b => b.UnitId == unitId);

            if (bu == null) return null;

            return new BloodUnitInventoryDto
            {
                UnitId = bu.UnitId,
                DonationId = bu.DonationId,
                BloodTypeId = bu.BloodTypeId,
                BloodTypeName = bu.BloodType.TypeName,
                ComponentId = bu.ComponentId,
                ComponentName = bu.Component.ComponentName,
                VolumeMl = bu.VolumeMl,
                CollectionDate = bu.CollectionDate,
                ExpirationDate = bu.ExpirationDate,
                StorageLocation = bu.StorageLocation,
                TestResults = bu.TestResults,
                Status = bu.Status,
                DiscardReason = bu.DiscardReason
            };
        }
    }
}