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

            if (profile == null)
                return null;

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
            var profile = new UserProfile
            {
                ProfileId = Guid.NewGuid().ToString(),
                UserId = dto.UserId,
                FullName = dto.FullName,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                Address = dto.Address,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
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

            profile.FullName = dto.FullName;
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
