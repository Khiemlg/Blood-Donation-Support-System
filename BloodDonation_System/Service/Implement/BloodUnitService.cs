// File: BloodDonation_System.Service.Implement.BloodUnitService.cs
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


        public async Task<bool> DiscardBloodUnitAsync(string bloodUnitId, string discardReason)
        {
            // Tìm đơn vị máu trong cơ sở dữ liệu
            var bloodUnit = await _context.BloodUnits.FirstOrDefaultAsync(b => b.UnitId == bloodUnitId);

            // Kiểm tra nếu không tìm thấy đơn vị máu hoặc trạng thái không phải "Available" hoặc "Reserved"
            if (bloodUnit == null || (bloodUnit.Status != "Available" && bloodUnit.Status != "Reserved"))
            {
                return false; // Trạng thái không hợp lệ để loại bỏ
            }

            // Cập nhật trạng thái và lý do loại bỏ
            bloodUnit.Status = "Discarded";
            bloodUnit.DiscardReason = discardReason;

            // Cập nhật vào cơ sở dữ liệu
            _context.BloodUnits.Update(bloodUnit);
            await _context.SaveChangesAsync();

            return true; // Thành công
        }














        /// <summary>
        /// Lấy tất cả các đơn vị máu với thông tin kho chi tiết (bao gồm tên BloodType và Component).
        /// </summary>
        /// <returns>Tập hợp các BloodUnitInventoryDto.</returns>
        public async Task<IEnumerable<BloodUnitInventoryDto>> GetAllAsync()
        {
            return await _context.BloodUnits
                .Include(bu => bu.BloodType)      // Tải BloodType để lấy tên
                .Include(bu => bu.Component)      // Tải BloodComponent để lấy tên
                .Select(bu => new BloodUnitInventoryDto // Ánh xạ sang BloodUnitInventoryDto
                {
                    UnitId = bu.UnitId,
                    DonationId = bu.DonationId,
                    BloodTypeId = bu.BloodTypeId,
                    BloodTypeName = bu.BloodType.TypeName, // Lấy tên từ BloodType
                    ComponentId = bu.ComponentId,
                    ComponentName = bu.Component.ComponentName, // Lấy tên từ BloodComponent
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
        /// Lấy một đơn vị máu theo ID với thông tin kho chi tiết.
        /// </summary>
        /// <param name="unitId">ID của đơn vị máu.</param>
        /// <returns>Một BloodUnitInventoryDto nếu tìm thấy, ngược lại là null.</returns>
        public async Task<BloodUnitInventoryDto?> GetByIdAsync(string unitId)
        {
            var bu = await _context.BloodUnits
                .Include(b => b.BloodType)      // Tải BloodType
                .Include(b => b.Component)      // Tải BloodComponent
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


        public async Task<BloodUnitInventoryDto> CreateAsync(BloodUnitDto dto)
        {
            // Kiểm tra các trường bắt buộc và khóa ngoại
            if (string.IsNullOrEmpty(dto.DonationId))
            {
                throw new ArgumentException("DonationId là bắt buộc để tạo một đơn vị máu mới.");
            }
            if (dto.BloodTypeId <= 0)
            {
                throw new ArgumentException("BloodTypeId là bắt buộc và phải là một giá trị hợp lệ.");
            }
            if (dto.ComponentId <= 0)
            {
                throw new ArgumentException("ComponentId là bắt buộc và phải là một giá trị hợp lệ.");
            }
            if (dto.VolumeMl <= 0)
            {
                throw new ArgumentException("VolumeMl phải lớn hơn 0.");
            }
            if (dto.CollectionDate == default) // Check if date is default (not set)
            {
                throw new ArgumentException("CollectionDate là bắt buộc và phải là một ngày hợp lệ.");
            }

            // Xác thực sự tồn tại của các khóa ngoại trong DB
            var donationExists = await _context.DonationHistories.AnyAsync(d => d.DonationId == dto.DonationId);
            if (!donationExists)
            {
                throw new ArgumentException($"DonationId '{dto.DonationId}' không tồn tại. Vui lòng đảm bảo nó là một ID hiến máu hợp lệ.");
            }
            var bloodTypeExists = await _context.BloodTypes.AnyAsync(bt => bt.BloodTypeId == dto.BloodTypeId);
            if (!bloodTypeExists)
            {
                throw new ArgumentException($"BloodTypeId '{dto.BloodTypeId}' không tồn tại. Vui lòng đảm bảo nó là một ID nhóm máu hợp lệ.");
            }
            var componentExists = await _context.BloodComponents.AnyAsync(bc => bc.ComponentId == dto.ComponentId);
            if (!componentExists)
            {
                throw new ArgumentException($"ComponentId '{dto.ComponentId}' không tồn tại. Vui lòng đảm bảo nó là một ID thành phần máu hợp lệ.");
            }

            // 1. Tính toán ExpirationDate dựa trên ComponentId
            DateOnly expirationDate;
            switch (dto.ComponentId)
            {
                case 1: // Hồng cầu (Red Blood Cells) - HSD 42 ngày
                    expirationDate = dto.CollectionDate.AddDays(42);
                    break;
                case 2: // Huyết tương (Plasma) - HSD 1 năm (đông lạnh)
                    expirationDate = dto.CollectionDate.AddYears(1);
                    break;
                case 3: // Tiểu cầu (Platelets) - HSD 5 ngày
                    expirationDate = dto.CollectionDate.AddDays(5);
                    break;
                case 4: // Máu toàn phần (Whole Blood) - HSD 35 ngày
                    expirationDate = dto.CollectionDate.AddDays(35);
                    break;
                default:
                    expirationDate = dto.CollectionDate.AddDays(30); // Giá trị mặc định
                    Console.WriteLine($"[CẢNH BÁO] ComponentId không xác định '{dto.ComponentId}'. Đang sử dụng ngày hết hạn mặc định (30 ngày).");
                    break;
            }

            // 2. Gán StorageLocation dựa trên ComponentId
            string assignedStorageLocation;
            switch (dto.ComponentId)
            {
                case 1: assignedStorageLocation = "COLD_STORAGE_A"; break; // Hồng cầu: Thường 1-6 độ C
                case 2: assignedStorageLocation = "FREEZER_ZONE_P"; break; // Huyết tương: -18 độ C hoặc lạnh hơn
                case 3: assignedStorageLocation = "AGITATOR_ROOM_T"; break; // Tiểu cầu: 20-24 độ C với khuấy liên tục
                case 4: assignedStorageLocation = "REFRIGERATED_CABINET_W"; break; // Máu toàn phần: 1-6 độ C
                default:
                    assignedStorageLocation = "GENERAL_STORAGE_UNKNOWN";
                    Console.WriteLine($"[CẢNH BÁO] ComponentId không xác định '{dto.ComponentId}'. Đang gán vị trí lưu trữ mặc định: {assignedStorageLocation}.");
                    break;
            }

            // 3. Tạo thực thể BloodUnit
            var entity = new BloodUnit
            {
                UnitId = "BUITS_" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper(), // Tạo UnitId duy nhất
                DonationId = dto.DonationId,
                BloodTypeId = dto.BloodTypeId,
                ComponentId = dto.ComponentId,
                VolumeMl = dto.VolumeMl,
                CollectionDate = dto.CollectionDate,
                ExpirationDate = expirationDate, // <-- Đã tính toán
                StorageLocation = assignedStorageLocation, // <-- Đã xác định
                TestResults = dto.TestResults ?? "Pending", // Mặc định là "Pending" nếu không được cung cấp
                Status = dto.Status ?? "Available", // Mặc định là "Available" nếu không được cung cấp
                DiscardReason = dto.DiscardReason // Thường là null khi tạo mới
            };

            try
            {
                _context.BloodUnits.Add(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.InnerException as Microsoft.Data.SqlClient.SqlException;
                if (sqlException != null && sqlException.Number == 2627) // Primary key violation (if UnitId somehow duplicates)
                {
                    throw new InvalidOperationException("Đã xảy ra lỗi khi tạo đơn vị máu: ID đã tồn tại.", ex);
                }
                if (sqlException != null && sqlException.Number == 547) // Foreign Key Constraint Violation or NOT NULL constraint
                {
                    throw new ArgumentException("Lỗi dữ liệu đầu vào: Một hoặc nhiều ID liên quan (Donation, BloodType, Component) không hợp lệ hoặc dữ liệu bắt buộc bị thiếu.", ex);
                }
                Console.Error.WriteLine($"[LỖI CSDL] Đã xảy ra lỗi khi tạo đơn vị máu: {ex.Message}");
                throw; // Ném lại ngoại lệ DbUpdateException chung
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[LỖI CHUNG] Đã xảy ra lỗi không mong muốn khi tạo đơn vị máu: {ex.Message}");
                throw;
            }

            // Tải lại thực thể với các navigation properties để trả về BloodUnitInventoryDto đầy đủ
            var createdAndLoadedEntity = await _context.BloodUnits
                .Include(bu => bu.BloodType)
                .Include(bu => bu.Component)
                .FirstOrDefaultAsync(bu => bu.UnitId == entity.UnitId);

            if (createdAndLoadedEntity == null)
            {
                throw new InvalidOperationException("Không thể truy xuất đơn vị máu vừa tạo sau khi lưu thành công.");
            }

            return new BloodUnitInventoryDto
            {
                UnitId = createdAndLoadedEntity.UnitId,
                DonationId = createdAndLoadedEntity.DonationId,
                BloodTypeId = createdAndLoadedEntity.BloodTypeId,
                BloodTypeName = createdAndLoadedEntity.BloodType.TypeName,
                ComponentId = createdAndLoadedEntity.ComponentId,
                ComponentName = createdAndLoadedEntity.Component.ComponentName,
                VolumeMl = createdAndLoadedEntity.VolumeMl,
                CollectionDate = createdAndLoadedEntity.CollectionDate,
                ExpirationDate = createdAndLoadedEntity.ExpirationDate,
                StorageLocation = createdAndLoadedEntity.StorageLocation,
                TestResults = createdAndLoadedEntity.TestResults,
                Status = createdAndLoadedEntity.Status,
                DiscardReason = createdAndLoadedEntity.DiscardReason
            };
        }

        
        public async Task<BloodUnitInventoryDto?> UpdateAsync(string unitId, BloodUnitDto dto)
        {
            var entity = await _context.BloodUnits.FirstOrDefaultAsync(bu => bu.UnitId.Equals(unitId));
            if (entity == null)
            {
                return null; // Không tìm thấy đơn vị máu
            }

            // Kiểm tra tính hợp lệ của các ID khóa ngoại nếu chúng có thể thay đổi
            // Chú ý: Các trường trong DTO được truyền vào có thể là null, cần xử lý tùy theo logic
            if (dto.DonationId != null) // Chỉ kiểm tra nếu DonationId được cung cấp để cập nhật
            {
                if (!await _context.DonationHistories.AnyAsync(d => d.DonationId == dto.DonationId))
                {
                    throw new ArgumentException($"DonationId '{dto.DonationId}' không tồn tại. Vui lòng đảm bảo nó là một ID hiến máu hợp lệ.");
                }
            }
            if (dto.BloodTypeId <= 0 || !await _context.BloodTypes.AnyAsync(bt => bt.BloodTypeId == dto.BloodTypeId))
            {
                throw new ArgumentException($"BloodTypeId '{dto.BloodTypeId}' không tồn tại hoặc không hợp lệ.");
            }
            if (dto.ComponentId <= 0 || !await _context.BloodComponents.AnyAsync(bc => bc.ComponentId == dto.ComponentId))
            {
                throw new ArgumentException($"ComponentId '{dto.ComponentId}' không tồn tại hoặc không hợp lệ.");
            }
            if (dto.VolumeMl <= 0)
            {
                throw new ArgumentException("VolumeMl phải lớn hơn 0.");
            }
            if (dto.CollectionDate == default)
            {
                throw new ArgumentException("CollectionDate là bắt buộc và phải là một ngày hợp lệ.");
            }


            // Cập nhật các trường từ DTO.
            // UnitId không được thay đổi.
            // Sử dụng null-coalescing cho các trường có thể là null trong DTO để cho phép cập nhật một phần.
            entity.DonationId = dto.DonationId ?? entity.DonationId;
            entity.BloodTypeId = dto.BloodTypeId;
            entity.ComponentId = dto.ComponentId;
            entity.VolumeMl = dto.VolumeMl;
            entity.CollectionDate = dto.CollectionDate;

            // ExpirationDate và StorageLocation được tính toán lại nếu ComponentId hoặc CollectionDate thay đổi
            DateOnly newExpirationDate;
            string newAssignedStorageLocation;

            switch (entity.ComponentId) // Sử dụng entity.ComponentId đã được cập nhật
            {
                case 1: newExpirationDate = entity.CollectionDate.AddDays(42); newAssignedStorageLocation = "COLD_STORAGE_A"; break;
                case 2: newExpirationDate = entity.CollectionDate.AddYears(1); newAssignedStorageLocation = "FREEZER_ZONE_P"; break;
                case 3: newExpirationDate = entity.CollectionDate.AddDays(5); newAssignedStorageLocation = "AGITATOR_ROOM_T"; break;
                case 4: newExpirationDate = entity.CollectionDate.AddDays(35); newAssignedStorageLocation = "REFRIGERATED_CABINET_W"; break;
                default:
                    newExpirationDate = entity.CollectionDate.AddDays(30);
                    newAssignedStorageLocation = "GENERAL_STORAGE_UNKNOWN";
                    Console.WriteLine($"[CẢNH BÁO] ComponentId không xác định '{entity.ComponentId}'. Đang sử dụng ngày hết hạn và vị trí lưu trữ mặc định.");
                    break;
            }
            entity.ExpirationDate = newExpirationDate;
            entity.StorageLocation = newAssignedStorageLocation; // Luôn gán lại theo ComponentId


            entity.TestResults = dto.TestResults ?? entity.TestResults;
            entity.Status = dto.Status ?? entity.Status;
            entity.DiscardReason = dto.DiscardReason ?? entity.DiscardReason;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.InnerException as Microsoft.Data.SqlClient.SqlException;
                if (sqlException != null && sqlException.Number == 547) // Foreign Key Constraint Violation or NOT NULL constraint
                {
                    throw new ArgumentException("Lỗi dữ liệu đầu vào: Một hoặc nhiều ID liên quan (Donation, BloodType, Component) không hợp lệ hoặc dữ liệu bắt buộc bị thiếu.", ex);
                }
                Console.Error.WriteLine($"[LỖI CSDL] Đã xảy ra lỗi không mong muốn khi cập nhật đơn vị máu ID {unitId}: {ex.Message}");
                throw;
            }
            catch (ArgumentException ex)
            {
                Console.Error.WriteLine($"[LỖI XÁC THỰC] Đã xảy ra lỗi xác thực đối số: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[LỖI CHUNG] Đã xảy ra lỗi không mong muốn khi cập nhật đơn vị máu ID {unitId}: {ex.Message}");
                throw;
            }

            // Tải lại bản ghi sau khi cập nhật với các mối quan hệ cần thiết để tạo DTO đầy đủ
            var updatedAndLoadedEntity = await _context.BloodUnits
                .Include(bu => bu.BloodType)
                .Include(bu => bu.Component)
                .FirstOrDefaultAsync(bu => bu.UnitId == unitId);

            if (updatedAndLoadedEntity == null)
            {
                // Trường hợp này hiếm khi xảy ra nếu FindAsync ban đầu thành công và SaveChangesAsync không gặp lỗi nghiêm trọng
                throw new InvalidOperationException("Không thể truy xuất đơn vị máu đã cập nhật.");
            }

            return new BloodUnitInventoryDto
            {
                UnitId = updatedAndLoadedEntity.UnitId,
                DonationId = updatedAndLoadedEntity.DonationId,
                BloodTypeId = updatedAndLoadedEntity.BloodTypeId,
                BloodTypeName = updatedAndLoadedEntity.BloodType.TypeName,
                ComponentId = updatedAndLoadedEntity.ComponentId,
                ComponentName = updatedAndLoadedEntity.Component.ComponentName,
                VolumeMl = updatedAndLoadedEntity.VolumeMl,
                CollectionDate = updatedAndLoadedEntity.CollectionDate,
                ExpirationDate = updatedAndLoadedEntity.ExpirationDate,
                StorageLocation = updatedAndLoadedEntity.StorageLocation,
                TestResults = updatedAndLoadedEntity.TestResults,
                Status = updatedAndLoadedEntity.Status,
                DiscardReason = updatedAndLoadedEntity.DiscardReason
            };
        }

        /// <summary>
        /// Xóa một đơn vị máu theo ID.
        /// </summary>
        /// <param name="unitId">ID của đơn vị máu cần xóa.</param>
        /// <returns>True nếu xóa thành công, ngược lại là false.</returns>
        /// <exception cref="InvalidOperationException">Ném ra nếu có lỗi cơ sở dữ liệu trong quá trình xóa.</exception>
        public async Task<bool> DeleteAsync(string unitId)
        {
            var entity = await _context.BloodUnits.FindAsync(unitId);
            if (entity == null) return false;

            try
            {
                _context.BloodUnits.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.InnerException as Microsoft.Data.SqlClient.SqlException;
                if (sqlException != null && sqlException.Number == 547) // Foreign Key Constraint Violation (if other entities reference this unit)
                {
                    throw new InvalidOperationException("Không thể xóa đơn vị máu này vì có các bản ghi khác đang tham chiếu đến nó (ví dụ: trong lịch sử sử dụng).", ex);
                }
                Console.Error.WriteLine($"[LỖI CSDL] Đã xảy ra lỗi khi xóa đơn vị máu ID {unitId}: {ex.Message}");
                throw new InvalidOperationException($"Lỗi cơ sở dữ liệu khi xóa đơn vị máu: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[LỖI CHUNG] Đã xảy ra lỗi không mong muốn khi xóa đơn vị máu ID {unitId}: {ex.Message}");
                throw;
            }
        }

     
        public async Task<IEnumerable<BloodUnitInventoryDto>> GetByBloodTypeIdAsync(int bloodTypeId)
        {
            if (bloodTypeId <= 0)
            {
                throw new ArgumentException("BloodTypeId phải là một giá trị hợp lệ lớn hơn 0.");
            }
            // Không cần kiểm tra sự tồn tại của BloodTypeId ở đây nếu bạn muốn trả về danh sách rỗng
            // nếu không có đơn vị nào thuộc nhóm máu đó hoặc nếu BloodTypeId không tồn tại.
            // Nếu bạn muốn ném lỗi nếu BloodTypeId không tồn tại, hãy thêm kiểm tra AnyAsync ở đây.

            return await _context.BloodUnits
                .Include(bu => bu.BloodType)
                .Include(bu => bu.Component)
                .Where(bu => bu.BloodTypeId == bloodTypeId)
                .Select(bu => new BloodUnitInventoryDto
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
                })
                .ToListAsync();
        }
    }
}