namespace DentalClinicPrototype.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string UserID { get; set; }
        public User User { get; set; }
        public string FullName { get; set; }
        public string ImageUrl { get; set; }
        public ICollection<MedicalRecord> MedicalRecords { get; set; }
    }
}
