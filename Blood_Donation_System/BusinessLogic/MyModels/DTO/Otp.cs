using System.Net.Mail;
using System.Net;

namespace Blood_Donation_System.BusinessLogic.MyModels.DTO
{
    public class Otp
    {
        public string GenerateOtp()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public async Task<bool> SendOtpEmail(string toEmail, string otp)
        {
            try
            {
                using (SmtpClient client = new SmtpClient("your_smtp_server_address")) // e.g., smtp.gmail.com
                {
                    client.Port = 587;
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential("your_email@example.com", "your_email_password");

                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress("your_email@example.com", "Your App Name");
                    mailMessage.To.Add(toEmail);
                    mailMessage.Subject = "Your OTP for Registration";
                    mailMessage.Body = $"Your One-Time Password (OTP) for registration is: <b>{otp}</b>. This OTP is valid for a short period.";
                    mailMessage.IsBodyHtml = true;

                    await client.SendMailAsync(mailMessage);
                    return true;
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }

    }
}
    
