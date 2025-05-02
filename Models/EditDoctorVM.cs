using System.ComponentModel.DataAnnotations;

namespace DentalClinicPrototype.Models
{
    public class EditDoctorVM
    {
        [Required(ErrorMessage = "Please enter your Full Name")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Please enter your User name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter your Email address")]
        [EmailAddress(ErrorMessage = "Please enter a valid Email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter your phone number")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string PhoneNumber { get; set; }
        public string? ImageUrl { get; set; }
        public IFormFile? Image { get; set; }
        public string Qualification { get; set; }
        public string Specialization { get; set; }
        public int Experience { get; set; }
        public int AppointmentPrice { get; set; }
    }
}
