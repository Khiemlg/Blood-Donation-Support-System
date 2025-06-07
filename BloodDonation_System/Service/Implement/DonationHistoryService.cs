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

        public async Task<IEnumerable<DonationHistoryDto>> GetAllAsync()
        {
            return await _context.DonationHistories
                .Select(dh => new DonationHistoryDto
                {
                    DonationId = dh.DonationId,
                    DonorUserId = dh.DonorUserId,
                    DonationDate = dh.DonationDate,
                    BloodTypeId = dh.BloodTypeId,
                    ComponentId = dh.ComponentId,
                    QuantityMl = dh.QuantityMl,
                    EligibilityStatus = dh.EligibilityStatus,
                    ReasonIneligible = dh.ReasonIneligible,
                    TestingResults = dh.TestingResults,
                    StaffUserId = dh.StaffUserId,
                    Status = dh.Status,
                    EmergencyId = dh.EmergencyId,
                    Descriptions = dh.Descriptions
                })
                .ToListAsync();
        }

        public async Task<DonationHistoryDto?> GetByIdAsync(string donationId)
        {
            var dh = await _context.DonationHistories.FindAsync(donationId);
            if (dh == null) return null;

            return new DonationHistoryDto
            {
                DonationId = dh.DonationId,
                DonorUserId = dh.DonorUserId,
                DonationDate = dh.DonationDate,
                BloodTypeId = dh.BloodTypeId,
                ComponentId = dh.ComponentId,
                QuantityMl = dh.QuantityMl,
                EligibilityStatus = dh.EligibilityStatus,
                ReasonIneligible = dh.ReasonIneligible,
                TestingResults = dh.TestingResults,
                StaffUserId = dh.StaffUserId,
                Status = dh.Status,
                EmergencyId = dh.EmergencyId,
                Descriptions = dh.Descriptions
            };
        }

        public async Task<DonationHistoryDto> CreateAsync(DonationHistoryDto dto)
        {
            var entity = new DonationHistory
            {
                DonationId = Guid.NewGuid().ToString(),
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
                Descriptions = dto.Descriptions
            };

            _context.DonationHistories.Add(entity);
            await _context.SaveChangesAsync();

            dto.DonationId = entity.DonationId;
            return dto;
        }

        public async Task<DonationHistoryDto?> UpdateAsync(string donationId, DonationHistoryDto dto)
        {
            var entity = await _context.DonationHistories.FindAsync(donationId);
            if (entity == null) return null;

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
            entity.EmergencyId = dto.EmergencyId;
            entity.Descriptions = dto.Descriptions;

            _context.DonationHistories.Update(entity);
            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<bool> DeleteAsync(string donationId)
        {
            var entity = await _context.DonationHistories.FindAsync(donationId);
            if (entity == null) return false;

            _context.DonationHistories.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
