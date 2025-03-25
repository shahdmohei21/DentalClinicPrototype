using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicPrototype.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        [ForeignKey("Doctor")]
        public int DoctorID { get; set; }
        [ForeignKey("Patient")]
        public int PatientID { get; set; }
        [ForeignKey("DoctorSchedule")]
        public int DoctorScheduleID { get; set; }
        public string ReasonForVisit { get; set; }
        public Doctor? Doctor { get; set; }
        public Patient? Patient { get; set; }
        public DoctorSchedule? DoctorSchedule { get; set; }
    }
}
