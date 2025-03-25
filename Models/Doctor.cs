namespace DentalClinicPrototype.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int AppointmentPrice { get; set; }
        public string Qualification { get; set; }
        public string ContactNumber { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; } 
        public string ImageUrl { get; set; }
        public string Specialization { get; set; }
        public ICollection<Nurse>? Nurses { get; set; }
        public ICollection<MedicalRecord>? MedicalRecords { get; set; }



    }
}
