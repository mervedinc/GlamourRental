using Microsoft.AspNetCore.Mvc;

namespace ClothingRental1.Controllers
{

    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
