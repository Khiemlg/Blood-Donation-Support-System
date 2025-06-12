using Microsoft.AspNetCore.Mvc;
using BloodDonation_System.Service.Interface;
using Microsoft.AspNetCore.Mvc;
namespace BloodDonation_System.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class DonationReminderController : ControllerBase
    {
        private readonly IDonationReminderService _service;

        public DonationReminderController(IDonationReminderService service)
        {
            _service = service;
        }

        [HttpPost("run-job")]
        public async Task<IActionResult> RunReminder()
        {
            await _service.RunDonationReminderJobAsync();
            return Ok("Reminder job executed.");
        }
    } 
    }
