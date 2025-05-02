using DentalClinicPrototype.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace DentalClinicPrototype.Controllers
{
    public class AdminHomeController : Controller
    {
        private readonly Context _context;

        public AdminHomeController(Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.DoctorCount = _context.Doctors.Count();
            ViewBag.PatientCount = _context.Patients.Count();
            ViewBag.TodayAppointments = _context.Appointments.Count();

            return View();
        }
    }
}
