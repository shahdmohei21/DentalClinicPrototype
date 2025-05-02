using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicPrototype.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public string Message { get; set; }
        [ForeignKey("Patient")]
        public int PatientID { get; set; }
        [ForeignKey("DoctorSchedule")]
        public int DoctorScheduleID { get; set; }
        public Patient? Patient { get; set; }
        public DoctorSchedule? DoctorSchedule { get; set; }

    }
}
