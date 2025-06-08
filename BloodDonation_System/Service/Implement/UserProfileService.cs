using BloodDonation_System.Data;
using BloodDonation_System.Model.DTO.UserProfile;
using BloodDonation_System.Model.Enties;
using BloodDonation_System.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation_System.Service.Implementation
{
    public class UserProfileService : IUserProfileService
    {
        private readonly DButils _context;

        public UserProfileService(DButils context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserProfileDto>> GetAllProfilesAsync()
        {
            return await _context.UserProfiles
                .Select(p => new UserProfileDto
                {
                    ProfileId = p.ProfileId,
                    UserId = p.UserId,
                    FullName = p.FullName,
                    DateOfBirth = p.DateOfBirth,
                    Gender = p.Gender,
                    Address = p.Address,
                    Latitude = p.Latitude,
                    Longitude = p.Longitude,
                    BloodTypeId = p.BloodTypeId,
                    RhFactor = p.RhFactor,
                    MedicalHistory = p.MedicalHistory,
                    LastBloodDonationDate = p.LastBloodDonationDate,
                    Cccd = p.Cccd,
                    PhoneNumber = p.PhoneNumber
                })
                .ToListAsync();
        }

        public async Task<UserProfileDto> GetProfileByIdAsync(string profileId)
        {
            var profile = await _context.UserProfiles.FindAsync(profileId);
            if (profile == null) return null;

            return new UserProfileDto
            {
                ProfileId = profile.ProfileId,
                UserId = profile.UserId,
                FullName = profile.FullName,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender,
                Address = profile.Address,
                Latitude = profile.Latitude,
                Longitude = profile.Longitude,
                BloodTypeId = profile.BloodTypeId,
                RhFactor = profile.RhFactor,
                MedicalHistory = profile.MedicalHistory,
                LastBloodDonationDate = profile.LastBloodDonationDate,
                Cccd = profile.Cccd,
                PhoneNumber = profile.PhoneNumber
            };
        }

        public async Task<UserProfileDto> CreateProfileAsync(CreateUserProfileDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.Cccd))
            {
                bool cccdExists = await _context.UserProfiles.AnyAsync(p => p.Cccd == dto.Cccd);
                if (cccdExists)
                    throw new InvalidOperationException("CCCD has been registered.");
            }

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                bool phoneExists = await _context.UserProfiles.AnyAsync(p => p.PhoneNumber == dto.PhoneNumber);
                if (phoneExists)
                    throw new InvalidOperationException("Phone number already in use.");
            }

            // ✅ Tạo ProfileId dạng PROFILE_0001 dựa trên ID hiện có lớn nhất
            var maxId = await _context.UserProfiles
                .Where(p => p.ProfileId.StartsWith("PROFILE_"))
                .OrderByDescending(p => p.ProfileId)
                .Select(p => p.ProfileId)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (!string.IsNullOrEmpty(maxId) &&
                int.TryParse(maxId.Substring("PROFILE_".Length), out int currentMax))
            {
                nextNumber = currentMax + 1;
            }

            string newId = $"PROFILE_{nextNumber:D4}";

            var profile = new UserProfile
            {
                ProfileId = newId,
                UserId = dto.UserId,
                FullName = dto.FullName ?? string.Empty,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                Address = dto.Address,
                Latitude = dto.Latitude ?? 0,
                Longitude = dto.Longitude ?? 0,
                BloodTypeId = dto.BloodTypeId,
                RhFactor = dto.RhFactor,
                MedicalHistory = dto.MedicalHistory,
                LastBloodDonationDate = dto.LastBloodDonationDate,
                Cccd = dto.Cccd,
                PhoneNumber = dto.PhoneNumber
            };

            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();

            return await GetProfileByIdAsync(profile.ProfileId);
        }

        public async Task<UserProfileDto> UpdateProfileAsync(string profileId, UpdateUserProfileDto dto)
        {
            var profile = await _context.UserProfiles.FindAsync(profileId);
            if (profile == null)
                return null;

            if (!string.IsNullOrWhiteSpace(dto.Cccd))
            {
                bool duplicateCccd = await _context.UserProfiles
                    .AnyAsync(p => p.Cccd == dto.Cccd && p.ProfileId != profileId);
                if (duplicateCccd)
                    throw new InvalidOperationException("CCCD has been registered.");
            }

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                bool duplicatePhone = await _context.UserProfiles
                    .AnyAsync(p => p.PhoneNumber == dto.PhoneNumber && p.ProfileId != profileId);
                if (duplicatePhone)
                    throw new InvalidOperationException("Phone number already in use.");
            }

            profile.FullName = dto.FullName ?? profile.FullName;
            profile.DateOfBirth = dto.DateOfBirth;
            profile.Gender = dto.Gender;
            profile.Address = dto.Address;
            profile.Latitude = dto.Latitude;
            profile.Longitude = dto.Longitude;
            profile.BloodTypeId = dto.BloodTypeId;
            profile.RhFactor = dto.RhFactor;
            profile.MedicalHistory = dto.MedicalHistory;
            profile.LastBloodDonationDate = dto.LastBloodDonationDate;
            profile.Cccd = dto.Cccd;
            profile.PhoneNumber = dto.PhoneNumber;

            await _context.SaveChangesAsync();

            return await GetProfileByIdAsync(profile.ProfileId);
        }

        public async Task<bool> DeleteProfileAsync(string profileId)
        {
            var profile = await _context.UserProfiles.FindAsync(profileId);
            if (profile == null)
                return false;

            _context.UserProfiles.Remove(profile);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
