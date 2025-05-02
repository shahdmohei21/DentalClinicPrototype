using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicPrototype.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        [ForeignKey("User")]
        public string UserID { get; set; }
        public User? User { get; set; }
        public string Qualification { get; set; }
        public string Specialization { get; set; }
        public int Experience { get; set; }
        public int appointmentPrice { get; set; }
        public ICollection<DoctorSchedule>? DoctorSchedules { get; set; }
        public ICollection<Review>? Reviews { get; set; }

        public ICollection<MedicalRecord>? MedicalRecords { get; set; }
    }
}
