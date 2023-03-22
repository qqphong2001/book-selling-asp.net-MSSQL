using bookstore.DbContext;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace bookstore.Areas.User.Controllers
{
    [Area("user")]
    public class ProductController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly IToastNotification _toastNotification;

        public ProductController(ApplicationDbContext db,IToastNotification toastNotification )
        {
            _db = db;
            
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("productDetail/{id}")]
        public IActionResult productDetail(int? id)
        {

            var result = _db.Books
            .Join(
                    _db.Authors,
                    book => book.author_id,
                    author => author.Id,
                    (book, author) => new { Book = book, Author = author.Name }
                 )
            .Join(
                 _db.Genres,
                 bookAuthor => bookAuthor.Book.genre_id,
                genre => genre.Id,
                (bookAuthor, genre) => new { Book = bookAuthor.Book, Author = bookAuthor.Author, Genre = genre.Name }
    )
            .Join(
                 _db.Publisher,
                bookAuthorGenre => bookAuthorGenre.Book.publisher_id,
                publisher => publisher.Id,
                (bookAuthorGenre, publisher) => new { Book = bookAuthorGenre.Book, Author = bookAuthorGenre.Author, Genre = bookAuthorGenre.Genre, Publisher = publisher.Name }
                )
                .Where(x => x.Book.Id == id)
                .FirstOrDefault();


            if (result == null)
            {
                _toastNotification.AddErrorToastMessage("sản phẩm không tồn tại");
                return RedirectToAction("Index","Home");
            }

            var thumbnail = _db.BookImages.Where(x => x.book_id == id).ToList();

            ViewBag.Thumbnail = thumbnail;
            ViewBag.book = result;

            var viewbook = _db.Books.Find(id);
            viewbook.view += 1;

            var bookgenre = _db.Books.
                Join(
                _db.Genres,
				 book => book.genre_id,
				genres => genres.Id,
					(book, genres) => new { Book = book, genres = genres.Name }
				).Join(
                _db.Authors,
                BookGenre => BookGenre.Book.author_id,
                author => author.Id,
                (BookGenre, author) => new {Book = BookGenre.Book , Genre = BookGenre.genres , Author = author.Name }
                )
                .Where(x => x.Book.genre_id == viewbook.genre_id).
                   Where(x => x.Book.Id != id).ToList()
				;

            ViewBag.BookGenre = bookgenre;  




            _db.SaveChanges();

            return View();
        }
    }
}
