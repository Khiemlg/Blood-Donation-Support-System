
using BloodDonation_System.Data; 
using BloodDonation_System.Model.DTO.Donation;
using BloodDonation_System.Model.Enties;
using BloodDonation_System.Service.Interface;
using Microsoft.EntityFrameworkCore;
using NETCore.MailKit.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace BloodDonation_System.Service.Implementation
{
    public class DonationRequestService : IDonationRequestService
    {
        private readonly DButils _context;

       
        private readonly Interface.IEmailService _emailService;

        public DonationRequestService(DButils context, Interface.IEmailService emailService)
        {
            _context = context;
            _emailService = emailService; 
        }


       
        public async Task<DonationRequestDto> CreateAsync(DonationRequestInputDto dto)
        {
            if (string.IsNullOrEmpty(dto.DonorUserId))
            {
                throw new ArgumentException("DonorUserId is required for creating a new donation request.");
            }

            var donationRequest = new DonationRequest
            {
                RequestId = "DONR_" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper(),
                DonorUserId = dto.DonorUserId, 
                BloodTypeId = dto.BloodTypeId,
                ComponentId = dto.ComponentId,
                PreferredDate = dto.PreferredDate,
                PreferredTimeSlot = dto.PreferredTimeSlot,
                Status = dto.Status ?? "Pending", 
                RequestDate = DateTime.UtcNow, 
                StaffNotes = dto.StaffNotes
            };

            await _context.DonationRequests.AddAsync(donationRequest);
            await _context.SaveChangesAsync();

            await _context.Entry(donationRequest)
                .Reference(dr => dr.BloodType).LoadAsync();
            await _context.Entry(donationRequest)
                .Reference(dr => dr.Component).LoadAsync();
            await _context.Entry(donationRequest)
                .Reference(dr => dr.DonorUser).Query()
                .Include(u => u.UserProfile)
                .LoadAsync();

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
                DonorUserName = donationRequest.DonorUser?.UserProfile?.FullName, 
                BloodTypeName = donationRequest.BloodType?.TypeName,
                ComponentName = donationRequest.Component?.ComponentName
            };
        }

        
        public async Task<IEnumerable<DonationRequestDto>> GetAllAsync()
        {
            return await _context.DonationRequests
                .Include(dr => dr.BloodType) 
                .Include(dr => dr.Component) 
                .Include(dr => dr.DonorUser) 
                    .ThenInclude(u => u.UserProfile) 
                .Select(dr => new DonationRequestDto 
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
                    DonorUserName = dr.DonorUser.UserProfile.FullName,
                    BloodTypeName = dr.BloodType.TypeName,
                    ComponentName = dr.Component.ComponentName
                })
                .ToListAsync();
        }

        
        public async Task<DonationRequestDto?> GetByIdAsync(string requestId)
        {
            var donationRequest = await _context.DonationRequests
                .Include(dr => dr.BloodType)
                .Include(dr => dr.Component)
                .Include(dr => dr.DonorUser)
                    .ThenInclude(u => u.UserProfile) 
                .FirstOrDefaultAsync(dr => dr.RequestId == requestId);

            if (donationRequest == null)
            {
                return null;
            }

           
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
                DonorUserName = donationRequest.DonorUser.UserProfile?.FullName, 
                BloodTypeName = donationRequest.BloodType.TypeName,
                ComponentName = donationRequest.Component.ComponentName
            };
        }

        public async Task<List<DonationRequestDto>> GetListByUserIdAsync(string userId)
        {
            var donationRequests = await _context.DonationRequests
                .Include(dr => dr.BloodType)
                .Include(dr => dr.Component)
                .Include(dr => dr.DonorUser)
                    .ThenInclude(u => u.UserProfile)
                .Where(dr => dr.DonorUserId == userId) // Filter by DonorUserId
                .ToListAsync(); // Get all matching results as a list

           
            return donationRequests.Select(dr => new DonationRequestDto
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
                DonorUserName = dr.DonorUser.UserProfile?.FullName,
                BloodTypeName = dr.BloodType.TypeName,
                ComponentName = dr.Component.ComponentName
            }).ToList();
        }


        public async Task<DonationRequestDto?> UpdateAsync(string requestId, DonationRequestInputDto dto)
        {
            var existingRequest = await _context.DonationRequests
                .FirstOrDefaultAsync(dr => dr.RequestId == requestId);

            if (existingRequest == null)
                return null;

            var oldStatus = existingRequest.Status?.ToLower();
            var newStatus = dto.Status?.ToLower();

            existingRequest.BloodTypeId = dto.BloodTypeId;
            existingRequest.ComponentId = dto.ComponentId;
            existingRequest.PreferredDate = dto.PreferredDate;
            existingRequest.PreferredTimeSlot = dto.PreferredTimeSlot;
            existingRequest.Status = dto.Status;
            existingRequest.StaffNotes = dto.StaffNotes;

            await _context.SaveChangesAsync();

            var donorEmail = await _context.Users
                .Where(u => u.UserId == existingRequest.DonorUserId)
                .Select(u => u.Email)
                .FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(donorEmail))
            {
                try
                {
                    if (newStatus.ToLower() == "accepted" && oldStatus.ToLower() != "accepted")
                    {
                        var subject = "Yêu cầu hiến máu đã được chấp nhận";
                        var body = $"Cảm ơn bạn đã đăng ký hiến máu. Yêu cầu của bạn đã được duyệt và sẽ tiến hành vào {existingRequest.PreferredDate:dd/MM/yyyy} lúc {existingRequest.PreferredTimeSlot}.";
                        await _emailService.SendEmailAsync(donorEmail, subject, body);
                    }
                    else if (newStatus == "rejected" && oldStatus != "rejected")
                    {
                        var subject = "Yêu cầu hiến máu bị từ chối";
                        var body = $"Rất tiếc, yêu cầu hiến máu của bạn đã bị từ chối vào {DateTime.Now:dd/MM/yyyy HH:mm} do : {dto.StaffNotes}. Vui lòng liên hệ để biết thêm chi tiết.";
                        await _emailService.SendEmailAsync(donorEmail, subject, body);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Email Error] RequestId {existingRequest.RequestId}: {ex.Message}");
                }
            }

            if (newStatus.ToLower() == "accepted" && oldStatus.ToLower() != "accepted")
            {
                try
                {
                    var donationHistory = new DonationHistory
                    {
                        DonationId = "HIST_" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper(),
                        DonationRequestId = existingRequest.RequestId,
                        DonorUserId = existingRequest.DonorUserId,
                        BloodTypeId = existingRequest.BloodTypeId,
                        ComponentId = existingRequest.ComponentId,
                        DonationDate = DateTime.UtcNow,
                        QuantityMl = 0, 
                        EligibilityStatus = "Eligible",
                        ReasonIneligible = null,
                        TestingResults = "Pending",
                        StaffUserId = null,
                        Status = "Pending",
                        EmergencyId = null,
                        Descriptions = $"Yêu cầu hiến máu ID {existingRequest.RequestId} đã được chấp nhận và ghi nhận lịch sử."
                    };

                    await _context.DonationHistories.AddAsync(donationHistory);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[History Error] RequestId {existingRequest.RequestId}: {ex.Message}");
                }
            }

            var updatedRequest = await _context.DonationRequests
    .Include(dr => dr.BloodType)
    .Include(dr => dr.Component)
    .Include(dr => dr.DonorUser)
        .ThenInclude(u => u.UserProfile)
    .FirstOrDefaultAsync(dr => dr.RequestId == requestId);

            if (updatedRequest == null)
            {
                // Optionally log the error or throw a custom exception
                return null;
            }

            return new DonationRequestDto
            {
                RequestId = updatedRequest.RequestId,
                DonorUserId = updatedRequest.DonorUserId,
                BloodTypeId = updatedRequest.BloodTypeId,
                ComponentId = updatedRequest.ComponentId,
                PreferredDate = updatedRequest.PreferredDate,
                PreferredTimeSlot = updatedRequest.PreferredTimeSlot,
                Status = updatedRequest.Status,
                RequestDate = updatedRequest.RequestDate,
                StaffNotes = updatedRequest.StaffNotes,
                DonorUserName = updatedRequest.DonorUser?.UserProfile?.FullName,
                BloodTypeName = updatedRequest.BloodType?.TypeName,
                ComponentName = updatedRequest.Component?.ComponentName
            };

        }





        public async Task<bool> DeleteAsync(string requestId)
        {
            var donationRequest = await _context.DonationRequests.FirstOrDefaultAsync(dr => dr.RequestId == requestId);
            if (donationRequest == null)
            {
                return false; 
            }

            _context.DonationRequests.Remove(donationRequest);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Dictionary<string, int>> GetSlotCountsByDateAsync(DateOnly date)
        {
            var timeSlots = new List<string>
            {
                "08:00 - 09:30",
                "09:30 - 11:00",
                "13:30 - 15:00",
                "15:00 - 16:30"
            };

         
            var requestsForDate = await _context.DonationRequests
                .Where(dr => dr.PreferredDate == date &&
                             (dr.Status == "Pending" || dr.Status == "Accepted")) 
                .ToListAsync();

            var slotCounts = new Dictionary<string, int>();
            foreach (var slot in timeSlots)
            {
                slotCounts[slot] = 0;
            }

            foreach (var request in requestsForDate)
            {
                if (!string.IsNullOrEmpty(request.PreferredTimeSlot) && slotCounts.ContainsKey(request.PreferredTimeSlot))
                {
                    slotCounts[request.PreferredTimeSlot]++;
                }
            }

            return slotCounts;
        }


    }
}