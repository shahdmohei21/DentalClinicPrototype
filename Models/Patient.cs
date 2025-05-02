namespace DentalClinicPrototype.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string UserID { get; set; }
        public User? User { get; set; }
        public ICollection<Review>? Reviews { get; set; }

        public ICollection<MedicalRecord> MedicalRecords { get; set; }
    }
}
