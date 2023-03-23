using bookstore.DbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bookstore.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("/Admin")]
    [Authorize(Roles = Role.Role_Admin)]
    public class HomeController : Controller
    {
        [Route("index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
