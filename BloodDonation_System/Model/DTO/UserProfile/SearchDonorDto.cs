namespace BloodDonation_System.Model.DTO.UserProfile
{
    public class SearchDonorDto
    {
        public int BloodTypeId { get; set; }
        public int ComponentId { get; set; }
        public double RadiusInKm { get; set; } // ✅ Đổi tên thành giống service

    }

}
