using bookstore.Areas.Admin.Models;
using bookstore.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Security.Policy;
using System.Text.RegularExpressions;

namespace bookstore.Areas.User.Controllers
{
    [Area("User")]

    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IToastNotification _toastNotification;
        private readonly SignInManager<bookstore.Areas.Admin.Models.UserModel> _SignInManager;
        private readonly UserManager<bookstore.Areas.Admin.Models.UserModel> _UserManager;

        public HomeController(ApplicationDbContext db,IToastNotification toastNotification, SignInManager<bookstore.Areas.Admin.Models.UserModel> SignInManager, UserManager<bookstore.Areas.Admin.Models.UserModel> UserManager)
        {
            _db = db;
            _toastNotification = toastNotification;

            _SignInManager = SignInManager;
            _UserManager = UserManager;
          
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
                .OrderBy(x => x.Book.view).ToList();
            ViewBag.book = bookmodel;

            var booksale = _db.Books.
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
              .Where(x => x.Book.discount > 0).ToList();
            ViewBag.sale = booksale;

            var bookBuy = _db.OrderDetails.Join(
                _db.Books,
                order => order.book_id,
                book => book.Id,
                (order, book) => new { order = order, book = book }
                ).GroupBy(x => x.order.book_id).Select(group =>
                new
                {
                    bookcount = group.Count(),
                    bookauthor = group.Select(x => x.book.author_id).FirstOrDefault(),
                    bookpublisher = group.Select(x => x.book.publisher_id).FirstOrDefault(),
                    bookpubtitle = group.Select(x => x.book.title).FirstOrDefault(),
                    bookdiscount = group.Select(x => x.book.discount).FirstOrDefault(),
                    bookdicover = group.Select(x => x.book.cover).FirstOrDefault(),
                    bookdiunitprice = group.Select(x => x.book.unitPrice).FirstOrDefault(),


                    bookgenre = group.Select(x => x.book.genre_id).FirstOrDefault(),

                    bookId = group.Key
                }
                ).Join(
                _db.Authors,
                book => book.bookauthor,
                author => author.Id,
                (book , author) => new { book = book , author = author.Name }
                ).Join(
                _db.Publisher,
                book => book.book.bookpublisher,
            publisher => publisher.Id,
                (book , publisher) => new {book = book , author = book.author ,publisher = publisher.Name }
                ).
                Join(
                _db.Genres,
                book => book.book.book.bookgenre,
                genre => genre.Id,
                (book , genre) => new { book = book , author = book.author , publisher = book.publisher , genre = genre.Name})
                .
                OrderBy(x =>  x.book.book.book.bookcount).Take(10)
                .ToList();
           
            
            ViewBag.bookBuy = bookBuy;

            ViewData["title"] = "Trang chủ";
            return View();
        
        }

        [Route("logout")]
        public async Task<IActionResult> logout()
        {
            await _SignInManager.SignOutAsync();

            return RedirectToAction("index");
        }

    }
}
