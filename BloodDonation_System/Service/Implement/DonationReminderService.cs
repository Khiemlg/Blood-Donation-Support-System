
using BloodDonation_System.Data;
using BloodDonation_System.Model.Enties;
using BloodDonation_System.Service.Interface;
using Microsoft.EntityFrameworkCore;






namespace BloodDonation_System.Service.Implement
{
 
    public class DonationReminderService : IDonationReminderService
    {
        private readonly DButils _context;
        private readonly IEmailService _emailService;

        public DonationReminderService(DButils context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task RunDonationReminderJobAsync()
        {
            var today = DateTime.UtcNow.Date;

            var profiles = await _context.UserProfiles
                .Where(p => p.LastBloodDonationDate != null)
                .ToListAsync();

            foreach (var profile in profiles)
            {
                var lastDate = profile.LastBloodDonationDate.Value.ToDateTime(TimeOnly.MinValue); 

                if ((DateTime.UtcNow.Date - lastDate.Date).TotalDays >= 90)

                {
                    bool alreadySent = await _context.ReminderLogs.AnyAsync(log =>
                        log.UserId == profile.UserId &&
                        log.ReminderType == "BloodDonation" &&
                        log.SentAt > lastDate
                    );

                    if (!alreadySent)
                    {
                        string message = "Hệ thống nhắc nhở bạn kiểm tra sức khỏe và sẵn sàng cho lần hiến máu tiếp theo.";

                        _context.Notifications.Add(new Notification
                        {
                            NotificationId = Guid.NewGuid().ToString(),
                            RecipientUserId = profile.UserId,
                            Message = message,
                            Type = "Reminder",
                            SentDate = DateTime.UtcNow,
                            IsRead = false
                        });

                        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == profile.UserId);
                        if (user != null && !string.IsNullOrEmpty(user.Email))
                        {
                            await _emailService.SendEmailAsync(
                                user.Email,
                                "Nhắc nhở hiến máu",
                                $"{profile.FullName}, đã đến lúc bạn có thể hiến máu trở lại. Hãy cùng giúp đỡ cộng đồng nhé!"
                            );
                        }

                        _context.ReminderLogs.Add(new ReminderLog
                        {
                            UserId = profile.UserId,
                            ReminderType = "BloodDonation",
                            SentAt = DateTime.UtcNow,
                            Via = "Both"
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
        }
    }

}
