using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicPrototype.Models
{
    public class MedicalRecord
    {
        public int Id { get; set; }
        [ForeignKey("Doctor")]
        public int DoctorId { get; set; }
        [ForeignKey("Patient")]
        public int PatientId { get; set; }
        public string Diagnosis { get; set; }
        public Doctor? Doctor { get; set; }
        public Patient? Patient { get; set; }
    }
}
