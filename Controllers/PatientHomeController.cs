using DentalClinicPrototype.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentalClinicPrototype.Controllers
{
    [Authorize(Roles = "Patient")]
    public class PatientHomeController : Controller
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;

        public PatientHomeController(Context context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<Patient?> GetCurrentPatientAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return await _context.Patients.FirstOrDefaultAsync(p => p.UserID == user.Id);
        }

        // ----------------- View All Doctors (with AJAX filtering) -----------------

        [AllowAnonymous]
        public async Task<IActionResult> AllDoctors()
        {
            var doctors = await _context.Doctors.Include(d => d.User).ToListAsync();
            return View(doctors);
        }

        [HttpGet]
        [AllowAnonymous]

        public IActionResult FilterDoctorsByName(string name)
        {
            var doctors = string.IsNullOrWhiteSpace(name)
                ? _context.Doctors.Include(d => d.User).ToList()
                : _context.Doctors
                    .Include(d => d.User)
                    .Where(d => d.User.FullName.Contains(name))
                    .ToList();

            return PartialView("_DoctorsPartial", doctors);
        }


        // ----------------- View Doctor Details -----------------
        [AllowAnonymous]

        public async Task<IActionResult> DoctorDetails(int id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Reviews)
                    .ThenInclude(r => r.Patient)
                        .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null) return NotFound();
            return View(doctor);
        }


        // ----------------- View Doctor Schedule -----------------
        [AllowAnonymous]

        public async Task<IActionResult> DoctorSchedule(int id)
        {
            var schedules = await _context.DoctorSchedules
                .Where(ds => ds.DoctorID == id && ds.ScheduleStatus == ScheduleStatus.Available)
                .ToListAsync();

            ViewBag.DoctorId = id;
            return View(schedules);
        }

        // ----------------- Book an Appointment -----------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookAppointment(int doctorScheduleId, string message)
        {
            var patient = await GetCurrentPatientAsync();
            if (patient == null) return Unauthorized();

            var schedule = await _context.DoctorSchedules.FindAsync(doctorScheduleId);
            if (schedule == null || schedule.ScheduleStatus != ScheduleStatus.Available)
                return NotFound();

            // Mark the schedule as booked
            schedule.ScheduleStatus = ScheduleStatus.Booked;

            // Create Appointment
            var appointment = new Appointment
            {
                PatientID = patient.Id,
                DoctorScheduleID = doctorScheduleId,
                Message = message
            };
            _context.Appointments.Add(appointment);

            // Create empty Medical Record
            var record = new MedicalRecord
            {
                DoctorId = schedule.DoctorID,
                PatientId = patient.Id,
                Diagnosis = ""
            };
            _context.MedicalRecords.Add(record);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyAppointments));
        }

        public async Task<IActionResult> MyAppointments()
        {
            var patient = await GetCurrentPatientAsync();
            if (patient == null) return Unauthorized();

            var appointments = await _context.Appointments
                .Include(a => a.DoctorSchedule)
                    .ThenInclude(ds => ds.Doctor)
                        .ThenInclude(d => d.User)
                .Where(a => a.PatientID == patient.Id)
                .ToListAsync();

            return View(appointments);
        }
        // ----------------- View Patient's Medical Record -----------------
        public async Task<IActionResult> ViewMedicalRecord()
        {
            var patient = await GetCurrentPatientAsync();
            if (patient == null) return Unauthorized();

            // Fetch the medical record for the current patient
            var medicalRecord = await _context.MedicalRecords
                .Include(mr => mr.Doctor)
                .ThenInclude(d => d.User)
                .Include(mr => mr.Patient)
                .Where(p=>p.PatientId== patient.Id)
                .ToListAsync();

            if (medicalRecord == null)
                return NotFound(); // If no record found, return 404

            return View(medicalRecord); // Return the view to display the record
        }
        // New action to get medical record details via AJAX
        [HttpGet]
        public async Task<IActionResult> GetMedicalRecord(int id)
        {
            var record = await _context.MedicalRecords
                .Include(r => r.Doctor).ThenInclude(d => d.User)
                .Include(r => r.Patient).ThenInclude(p => p.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (record == null)
            {
                return NotFound();
            }

            return Json(new
            {
                id = record.Id,
                date = record.CreatedAt.ToString("MMM dd, yyyy - hh:mm tt"),
                doctorName = $"Dr. {record.Doctor?.User?.FullName} ({record.Doctor?.Specialization})",
                patientName = record.Patient?.User?.FullName ?? "Current Patient",
                diagnosis = record.Diagnosis ?? "No diagnosis information provided."
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> AddReview(Review review)
        {
            var patient = await GetCurrentPatientAsync();
            if (patient == null) return Unauthorized();

            review.PatientID = patient.Id;
            review.CreatedAt = DateTime.Now;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("DoctorDetails", new { id = review.DoctorID });
        }

    }
}
