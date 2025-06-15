namespace BloodDonation_System.Model.DTO.Blood
{
    public class BloodTypeDto
    {
        public int BloodTypeId { get; set; }
        public string TypeName { get; set; } = null!;
        public string? Description { get; set; }
    }
}
