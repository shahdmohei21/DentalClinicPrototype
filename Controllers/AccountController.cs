using Microsoft.AspNetCore.Mvc;

namespace DentalClinicPrototype.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult signin()
        {
            return View();
        }
    }
}
