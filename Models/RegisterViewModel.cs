using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DentalClinicPrototype.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        // هنا تضيف الخاصية المطلوبة
        [Required(ErrorMessage = "Profile Image is required.")]
        public IFormFile Image { get; set; }
    }
}
