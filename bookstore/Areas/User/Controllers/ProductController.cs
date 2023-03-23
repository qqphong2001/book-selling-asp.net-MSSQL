using bookstore.Areas.Admin.Models;
using bookstore.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace bookstore.Areas.User.Controllers
{
    [Area("user")]
    public class ProductController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly IToastNotification _toastNotification;
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ProductController(ApplicationDbContext db,IToastNotification toastNotification, RoleManager<IdentityRole> roleManager, UserManager<UserModel> userManager)
        {
            _db = db;
            _toastNotification = toastNotification;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [Route("index")]
        public IActionResult Index()
        {
            ViewData["title"] = "Trang sản phẩm";


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
            ).ToList();

            ViewBag.books = result;



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
                ViewData["title"] = "Trang chủ";

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
                   Where(x => x.Book.Id != id).ToList();

            ViewBag.BookGenre = bookgenre;

            ViewData["title"] = "Chi tiết sản phẩm";



            _db.SaveChanges();

            return View();
        }


        public IActionResult productAdvance()
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
       ).ToList();

            ViewBag.bookAll = result;


            return View();
        }
        [Route("searchProduct")]
        [HttpPost]
        public IActionResult searchProduct([Bind("title")] BookModel obj )
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
      ).Where(x => x.Book.title.Contains(obj.title)).ToList();

            ViewBag.bookAll = result;


            return View("productAdvance");

        }




        [Route("SeedData")]
        public async Task<IActionResult> SeedData()
        {
            var rolenames = typeof(Role).GetFields().ToList();
            foreach (var r in rolenames)
            {
                var rolename = (string)r.GetRawConstantValue();
                var rfound = await _roleManager.FindByNameAsync(rolename);
                if (rfound == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(rolename));
                }
            }

            var userAdmin = await _userManager.FindByNameAsync(("Admin"));
            if (userAdmin == null)
            {
                userAdmin = new UserModel()
                {
                    UserName = "Admin123@gmail.com",
                    Email = "Admin123@gmail.com",
                    EmailConfirmed = true,

                };
                await _userManager.CreateAsync(userAdmin, "admin123");
                await _userManager.AddToRoleAsync(userAdmin, Role.Role_Admin);
            }
            _toastNotification.AddSuccessToastMessage("xong roi do");
            return RedirectToAction("index");
        }
    }
}
