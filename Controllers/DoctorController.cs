using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DentalClinicPrototype.Models;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace DentalClinicPrototype.Controllers
{

    [Authorize(Roles = "Admin")]
    public class DoctorController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly Context _context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public DoctorController(UserManager<User> userManager, Context context, IWebHostEnvironment webHostEnvironment)
        {
            this.userManager = userManager;
            _context = context;
            this.webHostEnvironment = webHostEnvironment;
        }

        // GET: Doctor/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Doctor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DoctorVM model)
        {
            if (ModelState.IsValid)
            {
                // Step 1: Create a new User for the doctor
                var doctorUser = new User
                {
                    UserName = model.UserName,  // Set username
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                };

                // Step 2: Create the User
                var result = await userManager.CreateAsync(doctorUser, model.Password);
                if (result.Succeeded)
                {
                    // Step 3: Assign the "Doctor" role to the User
                    await userManager.AddToRoleAsync(doctorUser, "Doctor");

                    // Step 4: Handle the image upload
                    string uniqueFileName = null;
                    if (model.Image != null)
                    {
                        string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images/users");
                        Directory.CreateDirectory(uploadsFolder);

                        uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.Image.CopyToAsync(fileStream);
                        }

                        doctorUser.ImageUrl = uniqueFileName;
                    }

                    // Step 5: Create the Doctor and associate it with the User
                    var doctor = new Doctor
                    {
                        UserID = doctorUser.Id,
                        Qualification = model.Qualification,
                        Specialization = model.Specialization,
                        Experience = model.Experience,
                        appointmentPrice = model.AppointmentPrice,
                    };

                    _context.Doctors.Add(doctor);
                    await _context.SaveChangesAsync();

                    // Redirect to list of doctors
                    return RedirectToAction("index");
                }

                // Add errors to the model if the creation failed
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        // GET: Doctor/Index
        public async Task<IActionResult> Index()
        {
            var doctors = await _context.Doctors.Include(d => d.User).ToListAsync();
            return View(doctors);
        }

        // GET: Doctor/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var doctor = await _context.Doctors.Include(d => d.User).FirstOrDefaultAsync(m => m.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // GET: Doctor/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var doctor = await _context.Doctors.Include(d => d.User).FirstOrDefaultAsync(m => m.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }

            var model = new EditDoctorVM
            {
                FullName = doctor.User.FullName,
                Email = doctor.User.Email,
                UserName = doctor.User.UserName,
                ImageUrl = doctor.User.ImageUrl,
                PhoneNumber = doctor.User.PhoneNumber,
                Qualification = doctor.Qualification,
                Specialization = doctor.Specialization,
                Experience = doctor.Experience,
                AppointmentPrice = doctor.appointmentPrice
            };

            return View(model);
        }

        // POST: Doctor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditDoctorVM model)
        {
            if (ModelState.IsValid)
            {
                var doctor = await _context.Doctors.Include(d => d.User).FirstOrDefaultAsync(m => m.User.Email == model.Email);
                if (doctor == null)
                {
                    return NotFound();
                }

                doctor.User.FullName = model.FullName;
                doctor.User.Email = model.Email;
                doctor.User.UserName = model.UserName;
                doctor.User.PhoneNumber = model.PhoneNumber;
                doctor.Qualification = model.Qualification;
                doctor.Specialization = model.Specialization;
                doctor.Experience = model.Experience;
                doctor.appointmentPrice = model.AppointmentPrice;

                // Handle image upload if new image is provided
                if (model.Image != null)
                {
                    string uniqueFileName = null;
                    if (!string.IsNullOrEmpty(doctor.User.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, "images/users", doctor.User.ImageUrl);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images/users");
                    Directory.CreateDirectory(uploadsFolder);

                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(fileStream);
                    }

                    doctor.User.ImageUrl = uniqueFileName;
                }

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorExists(doctor.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Doctor/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var doctor = await _context.Doctors.Include(d => d.User).FirstOrDefaultAsync(m => m.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // POST: Doctor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctor = await _context.Doctors.Include(d => d.User).FirstOrDefaultAsync(m => m.Id == id);
            if (doctor != null)
            {
                // Remove doctor's image file if exists
                if (!string.IsNullOrEmpty(doctor.User.ImageUrl))
                {
                    var imagePath = Path.Combine(webHostEnvironment.WebRootPath, "images/users", doctor.User.ImageUrl);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                // Remove the doctor and the associated user
                _context.Doctors.Remove(doctor);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool DoctorExists(int id)
        {
            return _context.Doctors.Any(e => e.Id == id);
        }
    }

}
