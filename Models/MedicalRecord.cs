using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicPrototype.Models
{
    public class MedicalRecord
    {
        public int Id { get; set; }
        [ForeignKey("Doctor")]
        public int DoctorID { get; set; }
        [ForeignKey("Patient")]
        public int PatientID { get; set; }
        public string Diagnosis { get; set; }
        public Doctor? Doctor { get; set; }
        public Patient? Patient { get; set; }
    }
}
