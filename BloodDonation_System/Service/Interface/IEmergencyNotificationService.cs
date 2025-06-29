﻿using BloodDonation_System.Model.DTO.Emergency;
using BloodDonation_System.Model.Enties;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloodDonation_System.Service.Interface
{
    public interface IEmergencyNotificationService
    {
        Task<IEnumerable<EmergencyNotificationDto>> GetAllAsync();
        Task<EmergencyNotificationDto?> GetByIdAsync(string notificationId);
        Task<EmergencyNotificationDto> CreateAsync(EmergencyNotificationDto dto);
        Task<EmergencyNotificationDto?> UpdateAsync(string notificationId, EmergencyNotificationDto dto);
        Task<bool> DeleteAsync(string notificationId);
        Task<string> RespondToEmergencyNotificationAsync(string userId, EmergencyResponseDTO dto);
        Task NotifyMatchingMembersAsync(EmergencyRequest request);

        Task<IEnumerable<EmergencyNotificationDto>> GetByEmergencyIdAsync(string emergencyId);
    }
}
