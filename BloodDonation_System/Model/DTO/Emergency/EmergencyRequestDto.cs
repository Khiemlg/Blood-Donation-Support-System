namespace BloodDonation_System.Model.DTO.Emergency
{
    public class EmergencyRequestDto
    {
        public string EmergencyId { get; set; } = null!;
        public string RequesterUserId { get; set; } = null!;
        public int BloodTypeId { get; set; }
        public int ComponentId { get; set; }
        public int QuantityNeededMl { get; set; }
        public string? Priority { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? FulfillmentDate { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
    }
}
