using Microsoft.EntityFrameworkCore;
using System;

namespace DentalClinicPrototype.Models
{
    public class context : DbContext
    {
        public context() : base() { }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Nurse> Nurses { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=MY-BOY;Initial Catalog=SystemPrototype;Integrated Security=True;TrustServerCertificate=True");
            base.OnConfiguring(optionsBuilder);
        }

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

            // Seed Data
            modelBuilder.Entity<Doctor>().HasData(
                new Doctor { Id = 1, FullName = "Dr. John Smith", AppointmentPrice = 100, Qualification = "DDS", ContactNumber = "1234567890", Gender = "Male", Email = "john.smith@example.com", PasswordHash = "hashed_password", ImageUrl = "image1.jpg", Specialization = "Orthodontist" },
                new Doctor { Id = 2, FullName = "Dr. Sarah Johnson", AppointmentPrice = 120, Qualification = "DMD", ContactNumber = "0987654321", Gender = "Female", Email = "sarah.johnson@example.com", PasswordHash = "hashed_password", ImageUrl = "image2.jpg", Specialization = "Periodontist" }
            );

            modelBuilder.Entity<Patient>().HasData(
                new Patient { Id = 1, FullName = "Alice Johnson", Email = "alice.johnson@example.com", PasswordHash = "hashed_password", Gender = "Female", ContactNumber = "1233211234", Address = "123 Street, NY", Age = 30 },
                new Patient { Id = 2, FullName = "Michael Brown", Email = "michael.brown@example.com", PasswordHash = "hashed_password", Gender = "Male", ContactNumber = "4445556666", Address = "456 Avenue, CA", Age = 25 }
            );

            modelBuilder.Entity<Nurse>().HasData(
                new Nurse { Id = 1, FullName = "Linda Carter", ContactNumber = "5551112222", Address = "456 Road, TX", Gender = "Female", Email = "linda.carter@example.com" },
                new Nurse { Id = 2, FullName = "Robert Green", ContactNumber = "7778889999", Address = "789 Blvd, FL", Gender = "Male", Email = "robert.green@example.com" }
            );

            modelBuilder.Entity<MedicalRecord>().HasData(
                new MedicalRecord { Id = 1, DoctorID = 1, PatientID = 1, Diagnosis = "Tooth decay" },
                new MedicalRecord { Id = 2, DoctorID = 2, PatientID = 2, Diagnosis = "Gum infection" }
            );

            DateTime staticDate = new DateTime(2025, 3, 23, 9, 0, 0);
            modelBuilder.Entity<DoctorSchedule>().HasData(
                new DoctorSchedule { Id = 1, DoctorID = 1, Status = true, ScheduleDateTime = staticDate.AddDays(1) },
                new DoctorSchedule { Id = 2, DoctorID = 2, Status = false, ScheduleDateTime = staticDate.AddDays(2) }
            );

            modelBuilder.Entity<Appointment>().HasData(
                new Appointment { Id = 1, DoctorID = 1, PatientID = 1, DoctorScheduleID = 1, ReasonForVisit = "Routine checkup" },
                new Appointment { Id = 2, DoctorID = 2, PatientID = 2, DoctorScheduleID = 2, ReasonForVisit = "Tooth extraction" }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
