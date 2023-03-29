using bookstore.Areas.Admin.Models;
using bookstore.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using NuGet.Protocol;

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

        [Route("productHot")]
        public IActionResult productHot()
        {
            var result = _db.Books.
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

            ViewBag.books = result;

            return View("index", "product");
        }
        [Route("productSale")]
        public IActionResult productSale()
        {
            var result = _db.Books.
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

            ViewBag.books = result;
            return View("index", "product");

        }

        [Route("productBuy")]
        public IActionResult productBuy()
        {
            var result = _db.OrderDetails.Join(
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
               (book, author) => new { book = book, author = author.Name }
               ).Join(
               _db.Publisher,
               book => book.book.bookpublisher,
           publisher => publisher.Id,
               (book, publisher) => new { book = book, author = book.author, publisher = publisher.Name }
               ).
               Join(
               _db.Genres,
               book => book.book.book.bookgenre,
               genre => genre.Id,
               (book, genre) => new { book = book, author = book.author, publisher = book.publisher, genre = genre.Name })
               .
               OrderBy(x => x.book.book.book.bookcount).Take(10)
               .ToList();
            ViewBag.books = result;
            return View("index", "product");
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

            var productHot = _db.Books.Join(
            _db.Genres,
            book => book.genre_id,
            genre => genre.Id,
            (book, genre) => new { book = book, Genre = genre }).OrderBy(x => x.book.view).Take(4).ToList();

            ViewBag.productHot = productHot;


            var genresWithBookCounts = _db.Genres
                 .Join(
                     _db.Books,
                     genre => genre.Id,
                     book => book.genre_id,
                     (genre, book) => new { genre = genre, book = book })
                 .GroupBy(x => x.genre.Name)
                 .Select(group => new {
                     GenreName = group.Key,
                     BookCount = group.Count(),
                     GenreId = group.Select(x => x.genre.Id).FirstOrDefault(),
                 })
                 .ToList();

            var publisherWithBookCounts = _db.Publisher.
                Join(
                  _db.Books,
                     publisher => publisher.Id,
                     book => book.publisher_id,
                     (publisher, book) => new { publisher = publisher, book = book }
                ).GroupBy(x => x.publisher.Name)
                .Select(group => new
                {
                    publisherName = group.Key,
                    BookCount = group.Count(),
                    publisherId = group.Select(x => x.publisher.Id).FirstOrDefault(),
                }

                )

                .ToList();



            ViewBag.genre = genresWithBookCounts;

            ViewBag.publisher = publisherWithBookCounts;






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


            var productHot = _db.Books.Join(
           _db.Genres,
           book => book.genre_id,
           genre => genre.Id,
           (book, genre) => new { book = book, Genre = genre }).OrderBy(x => x.book.view).Take(4).ToList();

            ViewBag.productHot = productHot;


            var genresWithBookCounts = _db.Genres
                   .Join(
                       _db.Books,
                       genre => genre.Id,
                       book => book.genre_id,
                       (genre, book) => new { genre = genre, book = book })
                   .GroupBy(x => x.genre.Name)
                   .Select(group => new {
                       GenreName = group.Key,
                       BookCount = group.Count(),
                       GenreId = group.Select(x => x.genre.Id).FirstOrDefault(),
                   })
                   .ToList();

            var publisherWithBookCounts = _db.Publisher.
                Join(
                  _db.Books,
                     publisher => publisher.Id,
                     book => book.publisher_id,
                     (publisher, book) => new { publisher = publisher, book = book }
                ).GroupBy(x => x.publisher.Name)
                .Select(group => new
                {
                    publisherName = group.Key,
                    BookCount = group.Count(),
                    publisherId = group.Select(x => x.publisher.Id).FirstOrDefault(),
                }

                )

                .ToList();


            ViewBag.genre = genresWithBookCounts;

            ViewBag.publisher = publisherWithBookCounts;

            return View("productAdvance");

        }


        [Route("genrebook/{id}")]
     
        public IActionResult genrebook(int? id)
        {

            var result = _db.Books
  .Join(
          _db.Authors,
          book => book.author_id,
          author => author.Id,
          (book, author) => new { Book = book, Author = author }
       )
  .Join(
       _db.Genres,
       bookAuthor => bookAuthor.Book.genre_id,
      genre => genre.Id,
      (bookAuthor, genre) => new { Book = bookAuthor.Book, Author = bookAuthor.Author, Genre = genre }
)
  .Join(
       _db.Publisher,
      bookAuthorGenre => bookAuthorGenre.Book.publisher_id,
      publisher => publisher.Id,
      (bookAuthorGenre, publisher) => new { Book = bookAuthorGenre.Book, Author = bookAuthorGenre.Author, Genre = bookAuthorGenre.Genre, Publisher = publisher }
      ).Where(x => x.Genre.Id == id).ToList();

            ViewBag.bookAll = result;



            var productHot = _db.Books.Join(
              _db.Genres,
              book => book.genre_id,
              genre => genre.Id,
              (book, genre) => new { book = book, Genre = genre }).OrderBy(x => x.book.view).Take(4).ToList();

            ViewBag.productHot = productHot;


            var genresWithBookCounts = _db.Genres
                .Join(
                    _db.Books,
                    genre => genre.Id,
                    book => book.genre_id,
                    (genre, book) => new { genre = genre, book = book })
                .GroupBy(x => x.genre.Name)
                .Select(group => new {
                    GenreName = group.Key,
                    BookCount = group.Count(),
                    GenreId = group.Select(x => x.genre.Id).FirstOrDefault(),
                })
                .ToList();

            var publisherWithBookCounts = _db.Publisher.
                Join(
                  _db.Books,
                     publisher => publisher.Id,
                     book => book.publisher_id,
                     (publisher, book) => new { publisher = publisher, book = book }
                ).GroupBy(x => x.publisher.Name)
                .Select(group => new
                {
                    publisherName = group.Key,
                    BookCount = group.Count(),
                    publisherId = group.Select(x => x.publisher.Id).FirstOrDefault(),
                }

                )

                .ToList();



            ViewBag.genre = genresWithBookCounts;

            ViewBag.publisher = publisherWithBookCounts;



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
