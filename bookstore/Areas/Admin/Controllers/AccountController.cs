using bookstore.Areas.Admin.Models;
using bookstore.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace bookstore.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/account")]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IToastNotification _toastNotification;
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<UserModel> _SignInManager;

        public AccountController(ApplicationDbContext db, IToastNotification toastNotification, RoleManager<IdentityRole> roleManager, UserManager<UserModel> userManager, SignInManager<UserModel> signInManager)
        {
            _db = db;
            _toastNotification = toastNotification;
            _roleManager = roleManager;
            _userManager = userManager;
            _SignInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Route("logout")]
        public async Task<IActionResult> logout()
        {
            await _SignInManager.SignOutAsync();

            IEnumerable<GenreModel> genremodel = _db.Genres.ToList();
            ViewBag.genre = genremodel;

            var bookmodel = _db.Books.
                Join(
                _db.Genres,
                 book => book.genre_id,
                genres => genres.Id,
                    (book, genres) => new { Book = book, genres = genres.Name }
                ).Join(
                _db.Authors,
                BookGenre => BookGenre.Book.author_id,
                author => author.Id,
                (BookGenre, author) => new { Book = BookGenre.Book, Genre = BookGenre.genres, Author = author.Name }
                )
                .ToList();
            ViewBag.book = bookmodel;

            ViewData["title"] = "Trang chủ";

            return View("~/Areas/User/Views/Home/Index.cshtml");
        }
    }
}
