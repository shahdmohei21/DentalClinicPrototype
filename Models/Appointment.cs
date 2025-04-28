namespace DentalClinicPrototype.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Message { get; set; } // Reason for visit

        public bool IsAccepted { get; set; } = false;

        // Foreign Key to Patient
        public int PatientID { get; set; }
        public Patient Patient { get; set; }

        // Foreign Key to Doctor (nullable because doctor accepts later)
        public int? DoctorID { get; set; }
        public Doctor? Doctor { get; set; }
    }
}
