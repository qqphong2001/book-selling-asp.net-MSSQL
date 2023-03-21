using Microsoft.AspNetCore.Mvc;

namespace bookstore.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("/Admin")]
    public class HomeController : Controller
    {
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
