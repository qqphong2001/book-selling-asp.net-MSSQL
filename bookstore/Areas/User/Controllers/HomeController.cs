using bookstore.Areas.Admin.Models;
using bookstore.DbContext;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace bookstore.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IToastNotification _toastNotification;
        public HomeController(ApplicationDbContext db,IToastNotification toastNotification)
        {
            _db = db;
            _toastNotification = toastNotification;
        }


        [Route("~/")]
        public IActionResult Index()
        {


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


            return View();
        }
    }
}
