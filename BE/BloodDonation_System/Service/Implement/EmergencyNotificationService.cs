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
    public class EmergencyNotificationService : IEmergencyNotificationService
    {
        private readonly DButils _context;

        public EmergencyNotificationService(DButils context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmergencyNotificationDto>> GetAllAsync()
        {
            return await _context.EmergencyNotifications
                .Select(en => new EmergencyNotificationDto
                {
                    NotificationId = en.NotificationId,
                    EmergencyId = en.EmergencyId,
                    RecipientUserId = en.RecipientUserId,
                    SentDate = en.SentDate,
                    DeliveryMethod = en.DeliveryMethod,
                    IsRead = en.IsRead,
                    ResponseStatus = en.ResponseStatus
                }).ToListAsync();
        }

        public async Task<EmergencyNotificationDto?> GetByIdAsync(string notificationId)
        {
            var en = await _context.EmergencyNotifications.FindAsync(notificationId);
            if (en == null) return null;

            return new EmergencyNotificationDto
            {
                NotificationId = en.NotificationId,
                EmergencyId = en.EmergencyId,
                RecipientUserId = en.RecipientUserId,
                SentDate = en.SentDate,
                DeliveryMethod = en.DeliveryMethod,
                IsRead = en.IsRead,
                ResponseStatus = en.ResponseStatus
            };
        }

        public async Task<EmergencyNotificationDto> CreateAsync(EmergencyNotificationDto dto)
        {
            var entity = new EmergencyNotification
            {
                NotificationId = Guid.NewGuid().ToString(),
                EmergencyId = dto.EmergencyId,
                RecipientUserId = dto.RecipientUserId,
                SentDate = dto.SentDate ?? DateTime.UtcNow,
                DeliveryMethod = dto.DeliveryMethod,
                IsRead = dto.IsRead,
                ResponseStatus = dto.ResponseStatus
            };

            _context.EmergencyNotifications.Add(entity);
            await _context.SaveChangesAsync();

            dto.NotificationId = entity.NotificationId;
            return dto;
        }

        public async Task<EmergencyNotificationDto?> UpdateAsync(string notificationId, EmergencyNotificationDto dto)
        {
            var entity = await _context.EmergencyNotifications.FindAsync(notificationId);
            if (entity == null) return null;

            entity.EmergencyId = dto.EmergencyId;
            entity.RecipientUserId = dto.RecipientUserId;
            entity.SentDate = dto.SentDate;
            entity.DeliveryMethod = dto.DeliveryMethod;
            entity.IsRead = dto.IsRead;
            entity.ResponseStatus = dto.ResponseStatus;

            _context.EmergencyNotifications.Update(entity);
            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<bool> DeleteAsync(string notificationId)
        {
            var entity = await _context.EmergencyNotifications.FindAsync(notificationId);
            if (entity == null) return false;

            _context.EmergencyNotifications.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
