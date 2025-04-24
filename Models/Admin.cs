using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicPrototype.Models
{
    public class Admin
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserID { get; set; }
        public User User { get; set; }
    }
}
