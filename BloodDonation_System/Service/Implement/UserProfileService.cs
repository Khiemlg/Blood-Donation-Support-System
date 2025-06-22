using BloodDonation_System.Data;
using BloodDonation_System.Model.DTO.UserProfile;
using BloodDonation_System.Model.Enties;

using BloodDonation_System.Service.Interface;
using BloodDonation_System.Utilities;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BloodDonation_System.Service.Implementation
{
    public class UserProfileService : IUserProfileService
    {
        private readonly DButils _context;
        private readonly GeocodingService _geocodingService;

        public UserProfileService(DButils context, GeocodingService geocodingService)
        {
            _context = context;
            _geocodingService = geocodingService;
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

        public async Task<UserProfileDto?> GetProfileByUserIdAsync(string userId)
        {
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
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

            var maxId = await _context.UserProfiles
                .Where(p => p.ProfileId.StartsWith("PROFILE_"))
                .OrderByDescending(p => p.ProfileId)
                .Select(p => p.ProfileId)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (!string.IsNullOrEmpty(maxId) && int.TryParse(maxId.Substring("PROFILE_".Length), out int currentMax))
            {
                nextNumber = currentMax + 1;
            }

            string newId = $"PROFILE_{nextNumber:D4}";

            var trimmedAddress = dto.Address?.Trim();
            var (lat, lon) = await _geocodingService.GetCoordinatesFromAddressAsync(trimmedAddress);

            if (lat == 0 && lon == 0)
                throw new InvalidOperationException("Địa chỉ không hợp lệ, vui lòng nhập chi tiết hơn (ít nhất quận/huyện và thành phố).");

            var profile = new UserProfile
            {
                ProfileId = newId,
                UserId = dto.UserId,
                FullName = dto.FullName ?? string.Empty,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                Address = trimmedAddress,
                Latitude = lat,
                Longitude = lon,
                BloodTypeId = dto.BloodTypeId,
                RhFactor = dto.RhFactor,
                MedicalHistory = dto.MedicalHistory,
                LastBloodDonationDate = dto.LastBloodDonationDate,
                Cccd = dto.Cccd,
                PhoneNumber = dto.PhoneNumber
            };

            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();

            return await GetProfileByUserIdAsync(dto.UserId);
        }

        public async Task<UserProfileDto?> UpdateProfileByUserIdAsync(string userId, UpdateUserProfileDto dto)
        {
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.Cccd))
            {
                bool duplicateCccd = await _context.UserProfiles.AnyAsync(p => p.Cccd == dto.Cccd && p.UserId != userId);
                if (duplicateCccd)
                    throw new InvalidOperationException("CCCD has been registered.");
            }

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                bool duplicatePhone = await _context.UserProfiles.AnyAsync(p => p.PhoneNumber == dto.PhoneNumber && p.UserId != userId);
                if (duplicatePhone)
                    throw new InvalidOperationException("Phone number already in use.");
            }

            if (!string.IsNullOrWhiteSpace(dto.FullName))
                profile.FullName = dto.FullName;
            if (dto.DateOfBirth.HasValue)
                profile.DateOfBirth = dto.DateOfBirth.Value;
            if (!string.IsNullOrWhiteSpace(dto.Gender))
                profile.Gender = dto.Gender;

            var trimmedAddress = dto.Address?.Trim();
            if (!string.IsNullOrWhiteSpace(trimmedAddress) && trimmedAddress != profile.Address)
            {
                var (lat, lon) = await _geocodingService.GetCoordinatesFromAddressAsync(trimmedAddress);
                if (lat == 0 && lon == 0)
                    throw new InvalidOperationException("Địa chỉ không hợp lệ, vui lòng nhập chi tiết hơn (ít nhất quận/huyện và thành phố).");

                profile.Address = trimmedAddress;
                profile.Latitude = lat;
                profile.Longitude = lon;
            }

            if (dto.BloodTypeId.HasValue)
                profile.BloodTypeId = dto.BloodTypeId.Value;
            if (!string.IsNullOrWhiteSpace(dto.RhFactor))
                profile.RhFactor = dto.RhFactor;
            if (!string.IsNullOrWhiteSpace(dto.MedicalHistory))
                profile.MedicalHistory = dto.MedicalHistory;
            if (dto.LastBloodDonationDate.HasValue)
                profile.LastBloodDonationDate = dto.LastBloodDonationDate.Value;
            if (!string.IsNullOrWhiteSpace(dto.Cccd))
                profile.Cccd = dto.Cccd;
            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                profile.PhoneNumber = dto.PhoneNumber;

            await _context.SaveChangesAsync();
            return await GetProfileByUserIdAsync(profile.UserId);

          

        }

        public async Task<bool> DeleteProfileByUserIdAsync(string userId)
        {
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile == null)
                return false;

            _context.UserProfiles.Remove(profile);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
