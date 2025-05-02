using DentalClinicPrototype.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace DentalClinicPrototype.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly Context db;

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager, IWebHostEnvironment webHostEnvironment,Context db)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.webHostEnvironment = webHostEnvironment;
            this.db = db;
        }

        //register
        public IActionResult Register()
        {
            return View();
        }

        //register post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM VM)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await userManager.FindByEmailAsync(VM.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email is already registered.");
                    return View(VM);
                }

                string uniqueFileName = null;

                // Handle image upload
                if (VM.Image != null)
                {
                    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images/users");
                    Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

                    uniqueFileName = Guid.NewGuid().ToString() + "_" + VM.Image.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await VM.Image.CopyToAsync(fileStream);
                    }
                }

                // Create a new user
                User user = new User
                {
                    UserName = VM.UserName,
                    Email = VM.Email,
                    PhoneNumber = VM.PhoneNumber,
                    FullName = VM.FullName,
                    ImageUrl = uniqueFileName,
                };

                var isGoodUser = await userManager.CreateAsync(user, VM.Password);
                if (isGoodUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Patient");

                    // Add new patient entry
                    var patient = new Patient
                    {
                        UserID = user.Id,
                        User = user
                    };

                    db.Patients.Add(patient);
                    await db.SaveChangesAsync();

                    return View("Login");
                }

                else
                {
                    foreach (var error in isGoodUser.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View(VM);
        }

        public IActionResult Login()
        {
            return View();
        }

        //login post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM VM)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(VM.Email);
                if (user != null)
                {
                    var isGoodUser = await userManager.CheckPasswordAsync(user, VM.Password);
                    if (isGoodUser)
                    {
                        // Build user claims manually
                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName ?? ""),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim("ImageUrl", user.ImageUrl ?? "")
                };

                        // Get user roles
                        var roles = await userManager.GetRolesAsync(user);
                        foreach (var role in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));
                        }

                        var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        // Sign in manually
                        await HttpContext.SignInAsync(
                            IdentityConstants.ApplicationScheme,
                            claimsPrincipal,
                            new AuthenticationProperties
                            {
                                IsPersistent = false // This ensures the cookie is not persistent
                            });

                        // Redirect based on role
                        if (roles.Contains("Admin"))
                        {
                            return RedirectToAction("Index", "AdminHome");
                        }
                        else if (roles.Contains("Doctor"))
                        {
                            return RedirectToAction("Index", "DoctorHome");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Wrong Data");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Wrong Data");
                }
            }
            return View(VM);
        }



        //logout
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "Home");
        }

        //access denied
        public IActionResult AccessDenied(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // Edit Profile View
        public async Task<IActionResult> EditProfile()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var profileVM = new EditProfileVM
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FullName = user.FullName,
                ImageUrl = user.ImageUrl
            };

            return View(profileVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileVM profileVM)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);

                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                user.UserName = profileVM.UserName;
                user.Email = profileVM.Email;
                user.PhoneNumber = profileVM.PhoneNumber;
                user.FullName = profileVM.FullName;

                if (profileVM.Image != null)
                {
                    // Delete old image
                    if (!string.IsNullOrEmpty(user.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, "images/users", user.ImageUrl);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Upload new image
                    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images/users");
                    Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + profileVM.Image.FileName;
                    string newFilePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(newFilePath, FileMode.Create))
                    {
                        await profileVM.Image.CopyToAsync(fileStream);
                    }

                    user.ImageUrl = uniqueFileName;
                }

                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Refresh claims
                    await HttpContext.SignOutAsync();

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Name, user.UserName ?? ""),
                        new Claim(ClaimTypes.Email, user.Email ?? ""),
                        new Claim("ImageUrl", user.ImageUrl ?? "")
                    };

                    // Get user roles
                    var roles = await userManager.GetRolesAsync(user);
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }
                    var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, claimsPrincipal);

                    // Redirect based on role
                    if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("Index", "AdminHome");
                    }
                    else if (roles.Contains("Doctor"))
                    {
                        return RedirectToAction("Index", "DoctorHome");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View(profileVM);
        }


    }
}
