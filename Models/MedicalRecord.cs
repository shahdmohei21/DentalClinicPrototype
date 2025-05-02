using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicPrototype.Models
{
    public class MedicalRecord
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Diagnosis { get; set; }
        public int Id { get; set; }
        [ForeignKey("Doctor")]
        public int DoctorId { get; set; }

        [ForeignKey("Patient")]
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
        public Doctor? Doctor { get; set; }

    }
}
