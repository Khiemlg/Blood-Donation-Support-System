using Blood_Donation_System.BusinessLogic.MyModels;
using Blood_Donation_System.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NETCore.MailKit.Core;

namespace Blood_Donation_System.Presentation.Controllers
{
    public class DonationController : Controller
    {
        private readonly DButils connect;
        private readonly IConfiguration Configuration;
        private readonly IEmailService EmailService;
        public DonationController(DButils C_in, IConfiguration configuration, IEmailService emailservice)
        {

            connect = C_in;
            Configuration = configuration;
            EmailService = emailservice;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("DonationRequest/List")]
        public async Task<ActionResult> Read()
        {
            return Ok(new { data = await connect.DonationRequests.ToListAsync() });
        }

        [HttpPost]
        [Route("DonationRequest/RegisterDonation")]

        public async Task<ActionResult> RegisterDonation(String id ,String userid, int typeBlood, int componentBlood, DateOnly PreferredDate, String slot)
        {
            var User = await connect.Users.FirstOrDefaultAsync(x => x.UserId.Equals(userid));
            if(User == null)
            {
                return BadRequest("user not found");
            }

            DonationRequest donation = new DonationRequest();
            string uniqueSuffix = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(); // Ví dụ: "D2A1C3B4"

            donation.RequestId = "BR_" + uniqueSuffix;
            donation.DonorUserId = userid;
            donation.BloodTypeId = typeBlood;
            donation.ComponentId = componentBlood;
            donation.PreferredDate = PreferredDate;
            donation.PreferredTimeSlot = slot;
            donation.Status = "Pending";
            connect.DonationRequests.Add(donation);
            return Ok(new { data = donation });
        }


    
    }
}