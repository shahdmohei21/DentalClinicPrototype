using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicPrototype.Models
{
    public class User : IdentityUser
    {
        public Doctor? Doctor { get; set; }
        public Patient? Patient { get; set; }
        public string FullName { get; set; }
        public string? ImageUrl { get; set; }
        [NotMapped]
        public IFormFile? Image { get; set; }
    }
}
