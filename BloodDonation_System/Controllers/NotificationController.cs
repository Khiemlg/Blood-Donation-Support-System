﻿using BloodDonation_System.Model.DTO.Notification;
using BloodDonation_System.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonation_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Staff,Member")]  // Có thể điều chỉnh thêm cho Member nếu cần
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET: api/notification
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _notificationService.GetAllNotificationsAsync();
            return Ok(result);
        }

        // GET: api/notification/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _notificationService.GetNotificationByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // POST: api/notification
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NotificationinputDto dto)
        {
            var created = await _notificationService.CreateNotificationAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.NotificationId }, created);
        }

        // PUT: api/notification/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] NotificationDto dto)
        {
            var updated = await _notificationService.UpdateNotificationAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        // DELETE: api/notification/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _notificationService.DeleteNotificationAsync(id);
            if (!success) return NotFound();
            return Ok("Deleted");
        }

        [HttpGet("by-user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<NotificationDto>), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotificationsForUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            try
            {
                var notifications = await _notificationService.GetNotificationsByRecipientUserIdAsync(userId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                // Log lỗi
                Console.WriteLine($"Error fetching notifications for user {userId}: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching notifications.");
            }
        }
    }
    }
