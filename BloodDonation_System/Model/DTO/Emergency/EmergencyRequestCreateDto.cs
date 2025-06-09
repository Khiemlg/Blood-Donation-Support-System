namespace BloodDonation_System.Model.DTO.Emergency
{
    public class EmergencyRequestCreateDto
    {
        public int BloodTypeId { get; set; }
        public int ComponentId { get; set; }
        public int QuantityNeededMl { get; set; }
        public string? Priority { get; set; } = "Normal";
        public DateTime DueDate { get; set; }
        public string? Description { get; set; }
    }
}
//EmergencyRequestCreateDto.cs dùng cho POST /create
//chỉ giữ lại các field cần nhập từ Staff: