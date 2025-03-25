namespace DentalClinicPrototype.Models
{
    public class Nurse
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string ContactNumber { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public ICollection<Doctor> ? Doctors { get; set; }
    }
}
