using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DentalClinicPrototype.Models
{
    public class Context : IdentityDbContext<User>
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey(a => a.PatientID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.DoctorSchedule)
                .WithMany()
                .HasForeignKey(a => a.DoctorScheduleID)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.Patient)
                .WithMany(p => p.MedicalRecords)
                .HasForeignKey(r => r.PatientId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.Doctor)
                .WithMany(d=>d.MedicalRecords)
                .HasForeignKey(r => r.DoctorId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Patient)
                .WithMany(p=>p.Reviews)
                .HasForeignKey(r => r.PatientID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Doctor)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.DoctorID)
                .OnDelete(DeleteBehavior.NoAction);
        }

    }
}
