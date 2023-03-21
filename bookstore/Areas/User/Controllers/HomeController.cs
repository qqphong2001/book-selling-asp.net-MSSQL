using Microsoft.AspNetCore.Mvc;

namespace bookstore.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
