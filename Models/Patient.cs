using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicPrototype.Models
{
    public class Patient
    {
        public int Id { get; set; }
        [ForeignKey("User")]
        public string UserID { get; set; }

        public User User { get; set; }
           public string Gender { get; set; }
        public string ImageUrl { get; set; }
        public string Address { get; set; }
        public int Age { get; set; }
        public ICollection<MedicalRecord>? MedicalRecords { get; set; }

    }
}
