// File: BloodDonation_System.Service.Implementation.DonationRequestService.cs
using BloodDonation_System.Data; // Giả định DbContext của bạn nằm ở đây (DButils)
using BloodDonation_System.Model.DTO.Donation;
using BloodDonation_System.Model.Enties;
using BloodDonation_System.Service.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BloodDonation_System.Service.Implementation
{
    // Triển khai IDonationRequestService
    public class DonationRequestService : IDonationRequestService
    {
        // DbContext để tương tác với cơ sở dữ liệu (đã đổi tên thành DButils)
        private readonly DButils _context;

        public DonationRequestService(DButils context)
        {
            _context = context;
        }

        /// <summary>
        /// Tạo một yêu cầu hiến máu mới từ Input DTO.
        /// </summary>
        /// <param name="dto">Đối tượng DonationRequestInputDto chứa thông tin yêu cầu hiến máu cần tạo.</param>
        /// <returns>DonationRequestResponseDto của yêu cầu đã tạo.</returns>
        public async Task<DonationRequestDto> CreateAsync(DonationRequestInputDto dto)
        {
            // Kiểm tra DonorUserId khi tạo mới là bắt buộc.
            // ArgumentException được ném ra nếu thiếu, để Controller có thể bắt và trả về BadRequest.
            if (string.IsNullOrEmpty(dto.DonorUserId))
            {
                throw new ArgumentException("DonorUserId is required for creating a new donation request.");
            }

            // Ánh xạ Input DTO sang thực thể DonationRequest
            var donationRequest = new DonationRequest
            {
                // Tạo RequestId duy nhất với tiền tố "DONR_"
                RequestId = "DONR_" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper(),
                DonorUserId = dto.DonorUserId, // Lấy từ DTO đầu vào
                BloodTypeId = dto.BloodTypeId,
                ComponentId = dto.ComponentId,
                PreferredDate = dto.PreferredDate,
                PreferredTimeSlot = dto.PreferredTimeSlot,
                Status = dto.Status ?? "Pending", // Mặc định trạng thái là "Pending"
                RequestDate = DateTime.UtcNow, // Ghi lại ngày yêu cầu
                StaffNotes = dto.StaffNotes
            };

            await _context.DonationRequests.AddAsync(donationRequest);
            await _context.SaveChangesAsync();

            // Tải các Navigation Properties cần thiết để tạo Response DTO đầy đủ
            // Bao gồm BloodType, Component và User.UserProfile
            await _context.Entry(donationRequest)
                .Reference(dr => dr.BloodType).LoadAsync();
            await _context.Entry(donationRequest)
                .Reference(dr => dr.Component).LoadAsync();
            await _context.Entry(donationRequest)
                .Reference(dr => dr.DonorUser).Query()
                .Include(u => u.UserProfile)
                .LoadAsync();

            // Ánh xạ thực thể đã tạo thành Response DTO để trả về
            return new DonationRequestDto
            {
                RequestId = donationRequest.RequestId,
                DonorUserId = donationRequest.DonorUserId,
                BloodTypeId = donationRequest.BloodTypeId,
                ComponentId = donationRequest.ComponentId,
                PreferredDate = donationRequest.PreferredDate,
                PreferredTimeSlot = donationRequest.PreferredTimeSlot,
                Status = donationRequest.Status,
                RequestDate = donationRequest.RequestDate,
                StaffNotes = donationRequest.StaffNotes,
                DonorUserName = donationRequest.DonorUser?.UserProfile?.FullName, // Truy cập FullName an toàn
                BloodTypeName = donationRequest.BloodType?.TypeName,
                ComponentName = donationRequest.Component?.ComponentName
            };
        }

        /// <summary>
        /// Lấy tất cả các yêu cầu hiến máu và ánh xạ chúng thành DonationRequestResponseDto.
        /// Bao gồm thông tin chi tiết về người hiến, nhóm máu và thành phần máu.
        /// </summary>
        /// <returns>Danh sách DonationRequestResponseDto.</returns>
        public async Task<IEnumerable<DonationRequestDto>> GetAllAsync()
        {
            return await _context.DonationRequests
                .Include(dr => dr.BloodType) // Bao gồm thông tin nhóm máu
                .Include(dr => dr.Component) // Bao gồm thông tin thành phần máu
                .Include(dr => dr.DonorUser) // Bao gồm thông tin người dùng
                    .ThenInclude(u => u.UserProfile) // Và thông tin hồ sơ người dùng để lấy FullName
                .Select(dr => new DonationRequestDto // Ánh xạ sang DonationRequestResponseDto
                {
                    RequestId = dr.RequestId,
                    DonorUserId = dr.DonorUserId,
                    BloodTypeId = dr.BloodTypeId,
                    ComponentId = dr.ComponentId,
                    PreferredDate = dr.PreferredDate,
                    PreferredTimeSlot = dr.PreferredTimeSlot,
                    Status = dr.Status,
                    RequestDate = dr.RequestDate,
                    StaffNotes = dr.StaffNotes,
                    DonorUserName = dr.DonorUser.UserProfile.FullName, // Truy cập FullName qua UserProfile
                    BloodTypeName = dr.BloodType.TypeName,
                    ComponentName = dr.Component.ComponentName
                })
                .ToListAsync();
        }

        /// <summary>
        /// Lấy một yêu cầu hiến máu theo ID và ánh xạ nó thành DonationRequestResponseDto.
        /// Bao gồm thông tin chi tiết về người hiến, nhóm máu và thành phần máu.
        /// </summary>
        /// <param name="requestId">ID của yêu cầu hiến máu.</param>
        /// <returns>DonationRequestResponseDto nếu tìm thấy, ngược lại là null.</returns>
        public async Task<DonationRequestDto?> GetByIdAsync(string requestId)
        {
            var donationRequest = await _context.DonationRequests
                .Include(dr => dr.BloodType)
                .Include(dr => dr.Component)
                .Include(dr => dr.DonorUser)
                    .ThenInclude(u => u.UserProfile) // Bao gồm UserProfile để lấy FullName
                .FirstOrDefaultAsync(dr => dr.RequestId == requestId);

            if (donationRequest == null)
            {
                return null;
            }

            // Ánh xạ thực thể tìm được thành Response DTO
            return new DonationRequestDto
            {
                RequestId = donationRequest.RequestId, // Đảm bảo RequestId được ánh xạ
                DonorUserId = donationRequest.DonorUserId,
                BloodTypeId = donationRequest.BloodTypeId,
                ComponentId = donationRequest.ComponentId,
                PreferredDate = donationRequest.PreferredDate,
                PreferredTimeSlot = donationRequest.PreferredTimeSlot,
                Status = donationRequest.Status,
                RequestDate = donationRequest.RequestDate,
                StaffNotes = donationRequest.StaffNotes,
                DonorUserName = donationRequest.DonorUser.UserProfile?.FullName, // Truy cập FullName an toàn
                BloodTypeName = donationRequest.BloodType.TypeName,
                ComponentName = donationRequest.Component.ComponentName
            };
        }

        /// <summary>
        /// Cập nhật thông tin của một yêu cầu hiến máu từ Input DTO.
        /// Tự động tạo DonationHistory nếu trạng thái chuyển sang "Completed".
        /// </summary>
        /// <param name="requestId">ID của yêu cầu hiến máu cần cập nhật.</param>
        /// <param name="dto">Đối tượng DonationRequestInputDto chứa thông tin cập nhật.</param>
        /// <returns>DonationRequestResponseDto của yêu cầu đã cập nhật, ngược lại là null nếu không tìm thấy.</returns>
        public async Task<DonationRequestDto?> UpdateAsync(string requestId, DonationRequestInputDto dto)
        {
            // Tìm yêu cầu hiện có trong cơ sở dữ liệu
            var existingRequest = await _context.DonationRequests
                .FirstOrDefaultAsync(dr => dr.RequestId == requestId);

            if (existingRequest == null)
            {
                return null; // Không tìm thấy yêu cầu
            }

            // Lưu trạng thái cũ của yêu cầu để kiểm tra sự thay đổi.
            var oldStatus = existingRequest.Status;

            // Cập nhật các trường từ Input DTO.
            // Lưu ý: DonorUserId không nên được cập nhật trong thao tác UPDATE này nếu nó là khóa hoặc không đổi.
            existingRequest.BloodTypeId = dto.BloodTypeId;
            existingRequest.ComponentId = dto.ComponentId;
            existingRequest.PreferredDate = dto.PreferredDate;
            existingRequest.PreferredTimeSlot = dto.PreferredTimeSlot;
            existingRequest.Status = dto.Status; // Cập nhật trạng thái
            existingRequest.StaffNotes = dto.StaffNotes;

            // Lưu các thay đổi của DonationRequest vào cơ sở dữ liệu trước.
            await _context.SaveChangesAsync();

            // --- Logic Tự động tạo DonationHistory khi trạng thái là "Completed" ---
            // Chỉ tạo lịch sử nếu trạng thái MỚI là "completed" và trạng thái CŨ KHÔNG phải là "completed"
            if (existingRequest.Status?.ToLower() == "completed" && oldStatus?.ToLower() != "completed")
            {
                try
                {
                    // Tạo một bản ghi DonationHistory mới từ thông tin của DonationRequest
                    var donationHistory = new DonationHistory
                    {
                        // Tạo DonationId duy nhất với tiền tố "HIST_"
                        DonationId = "HIST_" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper(),
                        DonationRequestId = existingRequest.RequestId, // Liên kết với yêu cầu hiến máu
                        DonorUserId = existingRequest.DonorUserId, // Lấy DonorUserId từ existingRequest
                        BloodTypeId = existingRequest.BloodTypeId,
                        ComponentId = existingRequest.ComponentId,
                        DonationDate = DateTime.UtcNow, // Thời điểm hiến máu hoàn thành
                        QuantityMl = 500, // Giá trị mặc định, cần cân nhắc nguồn dữ liệu thực tế
                        EligibilityStatus = "Eligible", // Trạng thái mặc định
                        ReasonIneligible = null,
                        TestingResults = "Pending", // Kết quả xét nghiệm thường có sau
                        StaffUserId = null, // Cần cập nhật sau nếu có thông tin staff
                        Status = "Pending", // Trạng thái của bản ghi lịch sử hiến máu
                        EmergencyId = null,
                        Descriptions = $"Yêu cầu hiến máu ID {existingRequest.RequestId} đã hoàn thành."
                    };

                    await _context.DonationHistories.AddAsync(donationHistory);
                    await _context.SaveChangesAsync(); // Lưu DonationHistory mới
                }
                catch (Exception ex)
                {
                    // Xử lý và log lỗi nếu việc tạo lịch sử thất bại.
                    Console.WriteLine($"Error creating donation history for request ID {existingRequest.RequestId}: {ex.Message}");
                    // Có thể xem xét rollback cập nhật DonationRequest nếu tạo lịch sử là bắt buộc.
                }
            }
            // --- Kết thúc Logic Tự động tạo DonationHistory ---

            // Tải lại các Navigation Properties để tạo Response DTO đầy đủ
            // Fetch lại để có data mới nhất và đầy đủ:
            var updatedAndLoadedRequest = await _context.DonationRequests
                .Include(dr => dr.BloodType)
                .Include(dr => dr.Component)
                .Include(dr => dr.DonorUser)
                    .ThenInclude(u => u.UserProfile)
                .FirstOrDefaultAsync(dr => dr.RequestId == requestId);

            // Ánh xạ thực thể đã cập nhật thành Response DTO
            return new DonationRequestDto
            {
                RequestId = updatedAndLoadedRequest.RequestId,
                DonorUserId = updatedAndLoadedRequest.DonorUserId,
                BloodTypeId = updatedAndLoadedRequest.BloodTypeId,
                ComponentId = updatedAndLoadedRequest.ComponentId,
                PreferredDate = updatedAndLoadedRequest.PreferredDate,
                PreferredTimeSlot = updatedAndLoadedRequest.PreferredTimeSlot,
                Status = updatedAndLoadedRequest.Status,
                RequestDate = updatedAndLoadedRequest.RequestDate,
                StaffNotes = updatedAndLoadedRequest.StaffNotes,
                DonorUserName = updatedAndLoadedRequest.DonorUser?.UserProfile?.FullName,
                BloodTypeName = updatedAndLoadedRequest.BloodType?.TypeName,
                ComponentName = updatedAndLoadedRequest.Component?.ComponentName
            };
        }

        /// <summary>
        /// Xóa một yêu cầu hiến máu theo ID.
        /// </summary>
        /// <param name="requestId">ID của yêu cầu hiến máu cần xóa.</param>
        /// <returns>True nếu xóa thành công, ngược lại là false.</returns>
        public async Task<bool> DeleteAsync(string requestId)
        {
            var donationRequest = await _context.DonationRequests.FirstOrDefaultAsync(dr => dr.RequestId == requestId);
            if (donationRequest == null)
            {
                return false; // Không tìm thấy yêu cầu
            }

            _context.DonationRequests.Remove(donationRequest);
            await _context.SaveChangesAsync();
            return true;
        }

        Task<DonationRequestDto?> IDonationRequestService.GetByIdAsync(string requestId)
        {
            throw new NotImplementedException();
        }
    }
}