namespace DentalClinicPrototype.Models
{
    public class MedicalRecord
    {
        public int Id { get; set; }

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public string Diagnosis { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
