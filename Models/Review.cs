using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicPrototype.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string ReviewText { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int Rating { get; set; } // Assuming a rating system from 1 to 5
        [ForeignKey("Doctor")]
        public int DoctorID { get; set; }
        [ForeignKey("Patient")]
        public int PatientID { get; set; }
        public Doctor? Doctor { get; set; }
        public Patient? Patient { get; set; }

    }
}
