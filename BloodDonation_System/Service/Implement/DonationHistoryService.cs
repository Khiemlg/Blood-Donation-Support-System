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
        private readonly IEmailService _emailService;


        public DonationHistoryService(DButils context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;

        }
      

        public async Task<IEnumerable<DonationHistoryDetailDto>> GetAllAsync()
        {
            return await _context.DonationHistories
                .Include(dh => dh.DonorUser)
                    .ThenInclude(u => u.UserProfile)
                .Include(dh => dh.BloodType)
                .Include(dh => dh.Component) 
                .Select(dh => new DonationHistoryDetailDto
                {
                    DonationId = dh.DonationId,
                    DonorUserId = dh.DonorUserId,
                    DonorUserName = dh.DonorUser.UserProfile.FullName,
                    DonationDate = dh.DonationDate,
                    BloodTypeId = dh.BloodTypeId,
                    BloodTypeName = dh.BloodType.TypeName,
                    ComponentId = dh.ComponentId,
                    ComponentName = dh.Component.ComponentName, 
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

            return dto;
        }



        public async Task<DonationHistoryDetailDto?> UpdateAsync(string donationId, DonationHistoryUpdateDto dto)
        {
            var donationHistory = await _context.DonationHistories
                .FirstOrDefaultAsync(dh => dh.DonationId.Equals(donationId));

            if (donationHistory == null)
            {
                return null; 
            }

            var oldStatus = donationHistory.Status;


            bool dtoProvidesDonationRequestId = !string.IsNullOrEmpty(dto.DonationRequestId);
            bool dtoProvidesEmergencyId = !string.IsNullOrEmpty(dto.EmergencyId);

            if (dtoProvidesDonationRequestId && dtoProvidesEmergencyId)
            {
                throw new ArgumentException("Không thể cung cấp cả DonationRequestId và EmergencyId. Chỉ một trong hai có thể được thiết lập.");
            }
            else if (dtoProvidesDonationRequestId)
            {
                var donationRequestExists = await _context.DonationRequests.AnyAsync(dr => dr.RequestId == dto.DonationRequestId);
                if (!donationRequestExists)
                {
                    throw new ArgumentException($"DonationRequestId '{dto.DonationRequestId}' không tồn tại.");
                }
                donationHistory.DonationRequestId = dto.DonationRequestId;
                donationHistory.EmergencyId = null; 
            }
            else if (dtoProvidesEmergencyId)
            {
                var emergencyExists = await _context.EmergencyRequests.AnyAsync(e => e.EmergencyId == dto.EmergencyId);
                if (!emergencyExists)
                {
                    throw new ArgumentException($"EmergencyId '{dto.EmergencyId}' không tồn tại.");
                }
                donationHistory.EmergencyId = dto.EmergencyId;
                donationHistory.DonationRequestId = null; 
            }
            else
            {
               
            }




            donationHistory.DonorUserId = dto.DonorUserId;
            donationHistory.DonationDate = dto.DonationDate;
            donationHistory.BloodTypeId = dto.BloodTypeId;
            donationHistory.ComponentId = dto.ComponentId;

            donationHistory.QuantityMl = dto.QuantityMl ?? donationHistory.QuantityMl;
            donationHistory.EligibilityStatus = dto.EligibilityStatus ?? donationHistory.EligibilityStatus;
            donationHistory.ReasonIneligible = dto.ReasonIneligible ?? donationHistory.ReasonIneligible;
            donationHistory.TestingResults = dto.TestingResults ?? donationHistory.TestingResults;
            donationHistory.StaffUserId = dto.StaffUserId ?? donationHistory.StaffUserId;
            donationHistory.Status = dto.Status ?? donationHistory.Status; 

            donationHistory.Descriptions = dto.Descriptions ?? donationHistory.Descriptions;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.InnerException as Microsoft.Data.SqlClient.SqlException;
                if (sqlException != null)
                {
                    if (sqlException.Number == 547) 
                    {
                        Console.Error.WriteLine($"[DB_ERROR] Foreign Key/NULL Constraint Violation (547). " +
                                                $"EmergencyId: '{donationHistory.EmergencyId ?? "NULL"}' " +
                                                $"DonationRequestId: '{donationHistory.DonationRequestId ?? "NULL"}'. " +
                                                $"SQL Error: {sqlException.Message}");
                        throw new InvalidOperationException("A foreign key or NOT NULL constraint was violated. " +
                                                            "This usually means an associated ID (Emergency, Donation Request, Donor, Staff) does not exist, " +
                                                            "OR you are trying to save NULL to a column that does not allow NULL in the database. " +
                                                            "Please ensure your database schema is up-to-date with your entity model (e.g., columns are nullable if intended).", ex);
                    }
                }
                Console.Error.WriteLine($"[DB_ERROR] An error occurred during database update: {ex.Message}");
                throw; 
            }
            catch (ArgumentException ex) 
            {
                Console.Error.WriteLine($"[VALIDATION_ERROR] {ex.Message}");
                throw;
            }
            catch (Exception ex) 
            {
                Console.Error.WriteLine($"[GENERAL_ERROR] An unexpected error occurred: {ex.Message}");
                throw;
            }

            var updatedEntity = await _context.DonationHistories
                .Include(dh => dh.DonorUser).ThenInclude(u => u.UserProfile)
                .Include(dh => dh.BloodType)
                .Include(dh => dh.Component)
                .Include(dh => dh.StaffUser).ThenInclude(u => u.UserProfile)
                .Include(dh => dh.Emergency) 
                .Include(dh => dh.DonationRequest) 
                .FirstOrDefaultAsync(dh => dh.DonationId == donationId);

            if (updatedEntity == null)
            {
                return null;
            }

            
            if (oldStatus?.ToLower() != "complete" && updatedEntity.Status?.ToLower() == "complete")
            {
                try
                {
                    DateOnly expirationDate;
                    int componentId = donationHistory.ComponentId; 

                    switch (componentId)
                    {
                        case 1: 
                            expirationDate = DateOnly.FromDateTime(donationHistory.DonationDate.AddDays(42));
                            break;
                        case 2: 
                            expirationDate = DateOnly.FromDateTime(donationHistory.DonationDate.AddYears(1));
                            break;
                        case 3: 
                            expirationDate = DateOnly.FromDateTime(donationHistory.DonationDate.AddDays(5));
                            break;
                        case 4:
                            expirationDate = DateOnly.FromDateTime(donationHistory.DonationDate.AddDays(35));
                            break;
                        default:
                            
                            expirationDate = DateOnly.FromDateTime(donationHistory.DonationDate.AddDays(30));
                            Console.WriteLine($"[WARNING] Unknown ComponentId {componentId} for DonationId {donationHistory.DonationId}. Using default expiration date (30 days).");
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
                        UnitId = "BUITS_" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper(),
                        DonationId = donationHistory.DonationId, 
                        BloodTypeId = donationHistory.BloodTypeId,
                        ComponentId = donationHistory.ComponentId,
                        VolumeMl = donationHistory.QuantityMl ?? 0, 
                        CollectionDate = DateOnly.FromDateTime(donationHistory.DonationDate),
                        ExpirationDate = expirationDate,
                        StorageLocation = assignedStorageLocation,
                        TestResults = donationHistory.TestingResults,
                        Status = "Pending", 
                        DiscardReason = "no" 
                    };

                    _context.BloodUnits.Add(newBloodUnit); 
                    Console.WriteLine($"[INFO] BloodUnit marked for creation for DonationId: {donationHistory.DonationId} with UnitId: {newBloodUnit.UnitId}.");
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"[ERROR] Failed to prepare BloodUnit for DonationId {donationHistory.DonationId}: {ex.Message}");
                    throw new InvalidOperationException($"Error preparing BloodUnit for creation: {ex.Message}", ex);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                if (!string.Equals(oldStatus, donationHistory.Status, StringComparison.OrdinalIgnoreCase))
                {
                    var email = await _context.Users
                        .Where(u => u.UserId == donationHistory.DonorUserId)
                        .Select(u => u.Email)
                        .FirstOrDefaultAsync();

                    if (!string.IsNullOrEmpty(email))
                    {
                        string subject = $"Trạng thái hiến máu đã được cập nhật: {donationHistory.Status}";
                        string body = donationHistory.Status?.ToLower() switch
                        {
                            "complete" => $"🩸 Xin chúc mừng! Quá trình hiến máu của bạn ngày {donationHistory.DonationDate:dd/MM/yyyy} đã hoàn tất. Cảm ơn bạn vì nghĩa cử cao đẹp!",
                            "pending" => $"⏳ Yêu cầu hiến máu của bạn đang ở trạng thái chờ xử lý. Vui lòng theo dõi cập nhật tiếp theo.",
                            "rejected" => $"❌ Rất tiếc! Yêu cầu hiến máu của bạn đã bị từ chối. Ghi chú: {donationHistory.ReasonIneligible ?? "Không rõ lý do"}.",
                            "ineligible" => $"⚠️ Bạn chưa đủ điều kiện hiến máu. Ghi chú: {donationHistory.ReasonIneligible ?? "Không rõ lý do"}.",
                            _ => $"ℹ️ Trạng thái hiến máu của bạn đã được cập nhật thành: {donationHistory.Status}."
                        };

                        try
                        {
                            await _emailService.SendEmailAsync(email, subject, body);
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine($"[EMAIL ERROR] Không gửi được email đến {email}: {ex.Message}");
                        }
                    }
                }



            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.InnerException as Microsoft.Data.SqlClient.SqlException;
                if (sqlException != null && sqlException.Number == 547) 
                {
                    Console.Error.WriteLine($"[DB_ERROR] Foreign Key/NULL Constraint Violation (547). " +
                                            $"SQL Error: {sqlException.Message}");
                    throw new InvalidOperationException("A foreign key or NOT NULL constraint was violated. " +
                                                        "This usually means an associated ID does not exist, " +
                                                        "OR you are trying to save NULL to a column that does not allow NULL. " +
                                                        "Please ensure your database schema is up-to-date with your entity model and all related IDs are valid.", ex);
                }
                Console.Error.WriteLine($"[DB_ERROR] An error occurred during database update for DonationHistory ID {donationId}: {ex.Message}");
                throw; 
            }
            catch (ArgumentException ex)
            {
                Console.Error.WriteLine($"[VALIDATION_ERROR] An argument validation error occurred: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[GENERAL_ERROR] An unexpected error occurred during DonationHistory update for ID {donationId}: {ex.Message}");
                throw;
            }

            var updatedAndLoadedEntity = await _context.DonationHistories
                .Include(dh => dh.DonorUser).ThenInclude(u => u.UserProfile)
                .Include(dh => dh.BloodType)
                .Include(dh => dh.Component)
                .Include(dh => dh.StaffUser).ThenInclude(u => u.UserProfile)
                .Include(dh => dh.Emergency)
                .Include(dh => dh.DonationRequest)
                .FirstOrDefaultAsync(dh => dh.DonationId == donationId);

            if (updatedAndLoadedEntity == null)
            {
                return null;
            }

            return new DonationHistoryDetailDto
            {
                DonationId = updatedAndLoadedEntity.DonationId,
                DonationRequestId = updatedAndLoadedEntity.DonationRequestId,
                DonorUserId = updatedAndLoadedEntity.DonorUserId,
                BloodTypeId = updatedAndLoadedEntity.BloodTypeId,
                ComponentId = updatedAndLoadedEntity.ComponentId,
                DonationDate = updatedAndLoadedEntity.DonationDate,
                QuantityMl = (int)updatedAndLoadedEntity.QuantityMl,
                EligibilityStatus = updatedAndLoadedEntity.EligibilityStatus,
                ReasonIneligible = updatedAndLoadedEntity.ReasonIneligible,
                TestingResults = updatedAndLoadedEntity.TestingResults,
                StaffUserId = updatedAndLoadedEntity.StaffUserId,
                Status = updatedAndLoadedEntity.Status,
                EmergencyId = updatedAndLoadedEntity.EmergencyId,
                Descriptions = updatedAndLoadedEntity.Descriptions,
                DonorUserName = updatedAndLoadedEntity.DonorUser?.UserProfile?.FullName,
                BloodTypeName = updatedAndLoadedEntity.BloodType?.TypeName,
                ComponentName = updatedAndLoadedEntity.Component?.ComponentName,
               
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
                .Where(dh => dh.DonorUserId == userId) 
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
                 
                })
                .ToListAsync();
        }
    }
}