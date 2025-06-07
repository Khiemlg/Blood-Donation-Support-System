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

        public async Task<BloodUnitDto> CreateAsync(BloodUnitDto dto)
        {
            var entity = new BloodUnit
            {
                UnitId = Guid.NewGuid().ToString(),
                DonationId = dto.DonationId,
                BloodTypeId = dto.BloodTypeId,
                ComponentId = dto.ComponentId,
                VolumeMl = dto.VolumeMl,
                CollectionDate = dto.CollectionDate,
                ExpirationDate = dto.ExpirationDate,
                StorageLocation = dto.StorageLocation,
                TestResults = dto.TestResults,
                Status = dto.Status,
                DiscardReason = dto.DiscardReason
            };

            _context.BloodUnits.Add(entity);
            await _context.SaveChangesAsync();

            dto.UnitId = entity.UnitId;
            return dto;
        }

        public async Task<BloodUnitDto?> UpdateAsync(string unitId, BloodUnitDto dto)
        {
            var entity = await _context.BloodUnits.FindAsync(unitId);
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

            _context.BloodUnits.Update(entity);
            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<bool> DeleteAsync(string unitId)
        {
            var entity = await _context.BloodUnits.FindAsync(unitId);
            if (entity == null) return false;

            _context.BloodUnits.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
