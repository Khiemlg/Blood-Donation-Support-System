namespace BloodDonation_System.Model.DTO.Blood
{
    public class BloodComponentDto
    {
        public int ComponentId { get; set; }
        public string ComponentName { get; set; } = null!;
        public string? Description { get; set; }
    }
}
