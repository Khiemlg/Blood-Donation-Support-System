namespace BloodDonation_System.Model.DTO.Blood
{
    public class BloodUnitDto
    {
     //   public string? UnitId { get; set; }
        public string? DonationId { get; set; }
        public int BloodTypeId { get; set; }
        public int ComponentId { get; set; }
        public int VolumeMl { get; set; }
        public DateOnly CollectionDate { get; set; }
        public DateOnly ExpirationDate { get; set; }
        public string? StorageLocation { get; set; }
        public string? TestResults { get; set; }
        public string? Status { get; set; }
        public string? DiscardReason { get; set; }
    }
}
