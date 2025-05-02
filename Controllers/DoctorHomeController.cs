using DentalClinicPrototype.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentalClinicPrototype.Controllers
{
   
    public class DoctorHomeController : Controller
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;

        public DoctorHomeController(Context context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<Doctor?> GetCurrentDoctorAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return await _context.Doctors.FirstOrDefaultAsync(d => d.UserID == user.Id);
        }

        // ------------------ DoctorSchedule CRUD ------------------

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Index()
        {
            var doctor = await GetCurrentDoctorAsync();
            if (doctor == null) return Unauthorized();

            var schedules = await _context.DoctorSchedules
                .Where(ds => ds.DoctorID == doctor.Id)
                .ToListAsync();

            return View(schedules);
        }

        public IActionResult CreateSchedule()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSchedule(DoctorSchedule schedule)
        {
            var doctor = await GetCurrentDoctorAsync();
            if (doctor == null) return Unauthorized();

            schedule.DoctorID = doctor.Id;
            schedule.ScheduleStatus = ScheduleStatus.Available;
            if (!ModelState.IsValid)
            {
                return View(schedule);
            }
            _context.DoctorSchedules.Add(schedule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EditSchedule(int id)
        {
            var doctor = await GetCurrentDoctorAsync();
            var schedule = await _context.DoctorSchedules.FindAsync(id);
            
            if (schedule == null || schedule.DoctorID != doctor?.Id)
                return NotFound();

            return View(schedule);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSchedule(DoctorSchedule schedule)
        {
            var doctor = await GetCurrentDoctorAsync();

            if (schedule.DoctorID != doctor?.Id)
                return Unauthorized();
            if (!ModelState.IsValid)
            {
                return View(schedule);
            }
            _context.Update(schedule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ------------------ MedicalRecord Read & Edit ------------------

        public async Task<IActionResult> MyPatients()
        {
            var doctor = await GetCurrentDoctorAsync();
            if (doctor == null) return Unauthorized();

            var records = await _context.MedicalRecords
                .Include(mr => mr.Patient)
                .ThenInclude(p => p.User)
                .Where(mr => mr.DoctorId == doctor.Id)
                .ToListAsync();

            return View(records);
        }

        public async Task<IActionResult> EditMedicalRecord(int id)
        {
            var doctor = await GetCurrentDoctorAsync();
            var record = await _context.MedicalRecords
                .Include(r => r.Patient)
                 .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (record == null || record.DoctorId != doctor?.Id)
                return NotFound();

            return View(record);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMedicalRecord(MedicalRecord record)
        {
            var doctor = await GetCurrentDoctorAsync();

            if (record.DoctorId != doctor?.Id)
                return Unauthorized();

            _context.Update(record);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyPatients));
        }
        // Inside DoctorController

        public async Task<IActionResult> MyAppointments()
        {
            var doctor = await GetCurrentDoctorAsync();
            if (doctor == null) return Unauthorized();

            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .ThenInclude(p => p.User)
                .Include(a => a.DoctorSchedule)
                .Where(a => a.DoctorSchedule!.DoctorID == doctor.Id)
                .ToListAsync();

            return View(appointments);
        }

    }
}
