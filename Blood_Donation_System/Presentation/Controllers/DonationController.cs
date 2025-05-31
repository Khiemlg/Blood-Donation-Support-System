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
        [Route ("DonationRequest/List")]
        public async Task<ActionResult> Read()
        {
            return Ok( new { data = await connect.DonationRequests.ToListAsync() } );
        }

        [HttpPost]
        [Route("DonationRequest/RegisterDonation")]

        public async Task<ActionResult> Register()
        {

        }


    }
}
