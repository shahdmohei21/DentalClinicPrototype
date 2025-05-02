using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicPrototype.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int Rating { get; set; } // Rating out of 5
        [Column(TypeName = "nvarchar(500)")]
        [Required(ErrorMessage = "Review text is required.")]
        [Display(Name = "Review Text")]
        [DataType(DataType.MultilineText)]
        [StringLength(500, ErrorMessage = "Review text cannot exceed 500 characters.")]
        public string ReviewText { get; set; } // Patient feedback

        [ForeignKey("Doctor")]
        public int DoctorID { get; set; }

        [ForeignKey("Patient")]
        public int PatientID { get; set; }
        public Patient? Patient { get; set; }
        public Doctor? Doctor { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
