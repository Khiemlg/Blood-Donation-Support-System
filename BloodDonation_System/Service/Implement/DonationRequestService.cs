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
    public class DonationRequestService : IDonationRequestService
    {
        private readonly DButils _context;

        public DonationRequestService(DButils context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DonationRequestDto>> GetAllAsync()
        {
            return await _context.DonationRequests
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
                    StaffNotes = dr.StaffNotes
                })
                .ToListAsync();
        }

        public async Task<DonationRequestDto?> GetByIdAsync(string requestId)
        {
            var dr = await _context.DonationRequests.FindAsync(requestId);
            if (dr == null) return null;

            return new DonationRequestDto
            {
                RequestId = dr.RequestId,
                DonorUserId = dr.DonorUserId,
                BloodTypeId = dr.BloodTypeId,
                ComponentId = dr.ComponentId,
                PreferredDate = dr.PreferredDate,
                PreferredTimeSlot = dr.PreferredTimeSlot,
                Status = dr.Status,
                RequestDate = dr.RequestDate,
                StaffNotes = dr.StaffNotes
            };
        }

        public async Task<DonationRequestDto> CreateAsync(DonationRequestDto dto)
        {
            var entity = new DonationRequest
            {
                RequestId = Guid.NewGuid().ToString(),
                DonorUserId = dto.DonorUserId,
                BloodTypeId = dto.BloodTypeId,
                ComponentId = dto.ComponentId,
                PreferredDate = dto.PreferredDate,
                PreferredTimeSlot = dto.PreferredTimeSlot,
                Status = dto.Status,
                RequestDate = dto.RequestDate ?? DateTime.UtcNow,
                StaffNotes = dto.StaffNotes
            };

            _context.DonationRequests.Add(entity);
            await _context.SaveChangesAsync();

            dto.RequestId = entity.RequestId;
            return dto;
        }

        public async Task<DonationRequestDto?> UpdateAsync(string requestId, DonationRequestDto dto)
        {
            var entity = await _context.DonationRequests.FindAsync(requestId);
            if (entity == null) return null;

            entity.DonorUserId = dto.DonorUserId;
            entity.BloodTypeId = dto.BloodTypeId;
            entity.ComponentId = dto.ComponentId;
            entity.PreferredDate = dto.PreferredDate;
            entity.PreferredTimeSlot = dto.PreferredTimeSlot;
            entity.Status = dto.Status;
            entity.RequestDate = dto.RequestDate;
            entity.StaffNotes = dto.StaffNotes;

            _context.DonationRequests.Update(entity);
            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<bool> DeleteAsync(string requestId)
        {
            var entity = await _context.DonationRequests.FindAsync(requestId);
            if (entity == null) return false;

            _context.DonationRequests.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
