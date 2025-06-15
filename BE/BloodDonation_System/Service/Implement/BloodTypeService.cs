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
    public class BloodTypeService : IBloodTypeService
    {
        private readonly DButils _context;

        public BloodTypeService(DButils context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BloodTypeDto>> GetAllAsync()
        {
            return await _context.BloodTypes
                .Select(bt => new BloodTypeDto
                {
                    BloodTypeId = bt.BloodTypeId,
                    TypeName = bt.TypeName,
                    Description = bt.Description
                }).ToListAsync();
        }

        public async Task<BloodTypeDto?> GetByIdAsync(int id)
        {
            var bt = await _context.BloodTypes.FindAsync(id);
            if (bt == null) return null;

            return new BloodTypeDto
            {
                BloodTypeId = bt.BloodTypeId,
                TypeName = bt.TypeName,
                Description = bt.Description
            };
        }

        public async Task<BloodTypeDto> CreateAsync(BloodTypeDto dto)
        {
            var entity = new BloodType
            {
                TypeName = dto.TypeName,
                Description = dto.Description
            };

            _context.BloodTypes.Add(entity);
            await _context.SaveChangesAsync();

            dto.BloodTypeId = entity.BloodTypeId;
            return dto;
        }

        public async Task<BloodTypeDto?> UpdateAsync(int id, BloodTypeDto dto)
        {
            var entity = await _context.BloodTypes.FindAsync(id);
            if (entity == null) return null;

            entity.TypeName = dto.TypeName;
            entity.Description = dto.Description;

            _context.BloodTypes.Update(entity);
            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.BloodTypes.FindAsync(id);
            if (entity == null) return false;

            _context.BloodTypes.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
