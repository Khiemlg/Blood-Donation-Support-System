using BloodDonation_System.Data;
using BloodDonation_System.Model.DTO.Emergency;
using BloodDonation_System.Model.Enties;
using BloodDonation_System.Service.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BloodDonation_System.Service.Implement
{
    public class EmergencyRequestService : IEmergencyRequestService
    {
        private readonly DButils _context;

        public EmergencyRequestService(DButils context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmergencyRequestDto>> GetAllAsync()
        {
            return await _context.EmergencyRequests
                .Select(er => new EmergencyRequestDto
                {
                    EmergencyId = er.EmergencyId,
                    RequesterUserId = er.RequesterUserId,
                    BloodTypeId = er.BloodTypeId,
                    ComponentId = er.ComponentId,
                    QuantityNeededMl = er.QuantityNeededMl,
                    Priority = er.Priority,
                    DueDate = er.DueDate,
                    CreationDate = er.CreationDate,
                    FulfillmentDate = er.FulfillmentDate,
                    Description = er.Description,
                    Status = er.Status
                }).ToListAsync();
        }

        public async Task<EmergencyRequestDto?> GetByIdAsync(string emergencyId)
        {
            var er = await _context.EmergencyRequests.FindAsync(emergencyId);
            if (er == null) return null;

            return new EmergencyRequestDto
            {
                EmergencyId = er.EmergencyId,
                RequesterUserId = er.RequesterUserId,
                BloodTypeId = er.BloodTypeId,
                ComponentId = er.ComponentId,
                QuantityNeededMl = er.QuantityNeededMl,
                Priority = er.Priority,
                DueDate = er.DueDate,
                CreationDate = er.CreationDate,
                FulfillmentDate = er.FulfillmentDate,
                Description = er.Description,
                Status = er.Status
            };
        }

        public async Task<EmergencyRequestDto> CreateAsync(EmergencyRequestDto dto)
        {
            var entity = new EmergencyRequest
            {
                EmergencyId = Guid.NewGuid().ToString(),
                RequesterUserId = dto.RequesterUserId,
                BloodTypeId = dto.BloodTypeId,
                ComponentId = dto.ComponentId,
                QuantityNeededMl = dto.QuantityNeededMl,
                Priority = dto.Priority,
                DueDate = dto.DueDate,
                CreationDate = dto.CreationDate ?? DateTime.UtcNow,
                FulfillmentDate = dto.FulfillmentDate,
                Description = dto.Description,
                Status = dto.Status
            };

            _context.EmergencyRequests.Add(entity);
            await _context.SaveChangesAsync();

            dto.EmergencyId = entity.EmergencyId;
            return dto;
        }

        public async Task<EmergencyRequestDto?> UpdateAsync(string emergencyId, EmergencyRequestDto dto)
        {
            var entity = await _context.EmergencyRequests.FindAsync(emergencyId);
            if (entity == null) return null;

            entity.RequesterUserId = dto.RequesterUserId;
            entity.BloodTypeId = dto.BloodTypeId;
            entity.ComponentId = dto.ComponentId;
            entity.QuantityNeededMl = dto.QuantityNeededMl;
            entity.Priority = dto.Priority;
            entity.DueDate = dto.DueDate;
            entity.CreationDate = dto.CreationDate;
            entity.FulfillmentDate = dto.FulfillmentDate;
            entity.Description = dto.Description;
            entity.Status = dto.Status;

            _context.EmergencyRequests.Update(entity);
            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<bool> DeleteAsync(string emergencyId)
        {
            var entity = await _context.EmergencyRequests.FindAsync(emergencyId);
            if (entity == null) return false;

            _context.EmergencyRequests.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
