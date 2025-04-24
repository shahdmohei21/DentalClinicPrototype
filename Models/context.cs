using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace DentalClinicPrototype.Models
{
    public class context : IdentityDbContext<User>
    {
        public context(DbContextOptions<context> options) : base(options) { }
       

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Nurse> Nurses { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Many-to-Many Relationship (Doctor - Nurse)
            modelBuilder.Entity<Doctor>()
                .HasMany(d => d.Nurses)
                .WithMany(n => n.Doctors);

            // Configure foreign key constraints with cascading delete
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany()
                .HasForeignKey(a => a.DoctorID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey(a => a.PatientID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.DoctorSchedule)
                .WithOne(d => d.Appointment)
                .HasForeignKey<Appointment>(a => a.DoctorScheduleID)
                .OnDelete(DeleteBehavior.NoAction);

                                                                         
 
            base.OnModelCreating(modelBuilder);
        }
    }
}
