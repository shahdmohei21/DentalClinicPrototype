using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicPrototype.Models
{
    public class DoctorSchedule
    {
        public int Id { get; set; }
        [ForeignKey("Doctor")]
        public int DoctorID { get; set; }
        public ScheduleStatus ScheduleStatus { get; set; } = ScheduleStatus.Available;
        public DateTime ScheduleDateTime { get; set; }
        public Doctor? Doctor { get; set; }
    }
        public enum ScheduleStatus { Available, Booked, Cancelled }
}
