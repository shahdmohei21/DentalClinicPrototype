using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicPrototype.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        [ForeignKey("User")]
        public string UserID { get; set; }

        public User User { get; set; }

        public string FullName { get; set; }
        
        public string ImageUrl { get; set; }
        public string Specialization { get; set; }
        //public ICollection<Nurse>? Nurses { get; set; }
        public ICollection<MedicalRecord>? MedicalRecords { get; set; }



    }
}
