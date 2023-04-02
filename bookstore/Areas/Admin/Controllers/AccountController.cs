using bookstore.Areas.Admin.Models;
using bookstore.DbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace bookstore.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/account")]
    [Authorize(Roles =Role.Role_Admin)]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IToastNotification _toastNotification;
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<UserModel> _SignInManager;
       
        public AccountController(ApplicationDbContext db, IToastNotification toastNotification, RoleManager<IdentityRole> roleManager,UserManager<UserModel> userManager, SignInManager<UserModel> signInManager)
        {
            _db = db;
            _toastNotification = toastNotification;
            _roleManager = roleManager;
            _userManager = userManager;
            _SignInManager = signInManager;
        }

        public async Task<IActionResult>  Index()
        {
           
            
            var usersWithRoles = await _userManager.Users.
     
             Join(
                _db.Customers,
                user => user.Id,
                customer => customer.account_id,
                (user,customer) => new { User = user, customer = customer })
             .ToListAsync();

            ViewBag.user = usersWithRoles;


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


            ViewBag.bookBuy = bookBuy;

            ViewData["title"] = "Trang chủ";

            return View("~/Areas/User/Views/Home/Index.cshtml");
        }

        [Route("create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> create()
        {
            var userAdmin = new UserModel()
            {
                UserName = (string) Request.Form["email"],

                Email = Request.Form["email"],
                EmailConfirmed = true,

            };
         
            var result = await _userManager.CreateAsync(userAdmin, Request.Form["password"]);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(userAdmin.UserName);
                await _userManager.AddToRoleAsync(user, Role.Role_Employee);
                var userId =  user.Id;

                var customer = new CustomerModel()
                {
                    dob = DateTime.Parse(Request.Form["dob"]),
                    firstName = Request.Form["firstName"],
                    lastName = Request.Form["lastName"],
                    gender = Int16.Parse(Request.Form["gender"]),
                    createdAt = DateTime.Now,
                    avatar = @"images/logo/logo.png",
                    phoneNumber = Request.Form["phone"],
                    account_id =  userId
                };

                _db.Customers.Add(customer);



                await _db.SaveChangesAsync();

                // ... rest of the code
            }

            return RedirectToAction("index");

        }

        

    }
}
