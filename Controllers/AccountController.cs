using DentalClinicPrototype.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DentalClinicPrototype.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET: Show Signin Page
        [HttpGet]
        public IActionResult Signin()
        {
            return View("signin"); // تأكد ان الفيو اسمه signin.cshtml في Views/Account
        }
        [HttpGet]
        public IActionResult signup()
        {
            return View("signup");
        }
        [HttpPost]
        public async Task<IActionResult> Signup(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string imagePath = null;
            if (model.Image != null && model.Image.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/users");
                Directory.CreateDirectory(uploadsFolder); // يتأكد أن المجلد موجود
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(fileStream);
                }

                imagePath = "/images/users/" + uniqueFileName; // هذا اللي بنخزنه
            }


            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                ImageUrl = imagePath // هذا مسار الصورة بعد ما تحفظها
            };



            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // تخزين الصورة لو موجودة
                if (model.Image != null)
                {
                    // ترفع الصورة وتحفظ الرابط في User.ImageUrl (لو تبي)
                }

                return RedirectToAction("Signin");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }


        // POST: Handle Signin Form
        [HttpPost]
        public async Task<IActionResult> Signin(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Try to find the user by email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check your email and password.");
                return View(model);
            }

            // Try to sign in (without RememberMe)
            var result = await _signInManager.PasswordSignInAsync(
                user.UserName,
                model.Password,
                isPersistent: false, // ما نحفظ الجلسة
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home"); // Successful login => go to Home
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check your email and password.");
            return View(model);
        }
  
    }



}
