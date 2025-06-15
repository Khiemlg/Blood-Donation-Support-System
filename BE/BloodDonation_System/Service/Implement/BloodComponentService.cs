using BloodDonation_System.Data;
using BloodDonation_System.Model.DTO.Blood;
using BloodDonation_System.Model.Enties;
using BloodDonation_System.Service.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BloodDonation_System.Service.Implement
{
    public class BloodComponentService : IBloodComponentService
    {
        private readonly DButils _context;

        public BloodComponentService(DButils context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BloodComponentDto>> GetAllAsync()
        {
            return await _context.BloodComponents
                .Select(c => new BloodComponentDto
                {
                    ComponentId = c.ComponentId,
                    ComponentName = c.ComponentName,
                    Description = c.Description
                }).ToListAsync();
        }

        public async Task<BloodComponentDto?> GetByIdAsync(int id)
        {
            var component = await _context.BloodComponents.FindAsync(id);
            if (component == null) return null;

            return new BloodComponentDto
            {
                ComponentId = component.ComponentId,
                ComponentName = component.ComponentName,
                Description = component.Description
            };
        }

        public async Task<BloodComponentDto> CreateAsync(BloodComponentDto dto)
        {
            var entity = new BloodComponent
            {
                ComponentName = dto.ComponentName,
                Description = dto.Description
            };

            _context.BloodComponents.Add(entity);
            await _context.SaveChangesAsync();

            dto.ComponentId = entity.ComponentId;
            return dto;
        }

        public async Task<BloodComponentDto?> UpdateAsync(int id, BloodComponentDto dto)
        {
            var entity = await _context.BloodComponents.FindAsync(id);
            if (entity == null) return null;

            entity.ComponentName = dto.ComponentName;
            entity.Description = dto.Description;

            _context.BloodComponents.Update(entity);
            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.BloodComponents.FindAsync(id);
            if (entity == null) return false;

            _context.BloodComponents.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
