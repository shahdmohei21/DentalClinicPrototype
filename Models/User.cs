using Microsoft.AspNetCore.Identity;

namespace DentalClinicPrototype.Models
{
    public class User : IdentityUser
    {
        public Admin? Admin { get; set; }
        public Doctor? Doctor { get; set; }
        public Patient? Patient { get; set; }

      
        public string? FullName { get; set; }
        public string? ImageUrl { get; set; } 
    }
}
