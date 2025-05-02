using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DentalClinicPrototype.Models;
using System.Threading.Tasks;
using System.Linq;

namespace DentalClinicPrototype.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PatientController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly Context _context;

        public PatientController(UserManager<User> userManager, Context context)
        {
            this.userManager = userManager;
            _context = context;
        }

        // GET: Patient/Index - View all patients
        public async Task<IActionResult> Index()
        {
            var patients = await _context.Patients.Include(p => p.User).ToListAsync();
            return View(patients);
        }

        // GET: Patient/Details/5 - View a specific patient's details including medical records and appointments
        public async Task<IActionResult> Details(int id)
        {
            var patient = await _context.Patients
                .Include(p => p.User)
                .Include(p => p.MedicalRecords)
                .ThenInclude(a => a.Doctor)
                .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null)
                return NotFound();

            return View(patient);
        }

        // GET: Patient/Appointments - View all appointments of all patients
        public async Task<IActionResult> Appointments()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.DoctorSchedule)
                    .ThenInclude(d => d.Doctor)
                    .ThenInclude(d=>d.User)
                .ToListAsync();

            return View(appointments);
        }
    }
}
