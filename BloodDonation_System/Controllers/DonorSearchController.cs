using Microsoft.AspNetCore.Mvc;

using BloodDonation_System.Model.DTO.UserProfile;
using System.Collections.Generic;
using System.Threading.Tasks;
using BloodDonation_System.Service.Implement;

namespace BloodDonation_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DonorSearchController : ControllerBase
    {
        private readonly DonorSearchService _donorSearchService;

        public DonorSearchController(DonorSearchService donorSearchService)
        {
            _donorSearchService = donorSearchService;
        }

        [HttpPost("search")]
        public async Task<ActionResult<List<UserProfileDto>>> SearchDonors([FromBody] SearchDonorDto request)
        {
            if (request.RadiusInKm <= 0 || request.BloodTypeId <= 0)
            {
                return BadRequest("Thông tin tìm kiếm không hợp lệ.");
            }

            var result = await _donorSearchService.SearchSuitableDonorsAsync(request);

            if (!result.Any())
            {
                return Ok(new
                {
                    message = "Không tìm thấy người hiến máu phù hợp.",
                    data = result
                });
            }
            return Ok(new
            {
                message = "Danh sách người hiến phù hợp.",
                data = result
            });



            
        }
    }
}
