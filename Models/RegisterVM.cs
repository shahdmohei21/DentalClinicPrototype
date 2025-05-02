using System.ComponentModel.DataAnnotations;

namespace DentalClinicPrototype.Models
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Please enter your Full Name")]
        [Display (Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Please enter your User name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter your Email address")]
        [EmailAddress(ErrorMessage = "Please enter a valid Email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please enter your phone number")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string PhoneNumber { get; set; }
        public string? ImageUrl { get; set; }
        public IFormFile? Image { get; set; }

    }
}
