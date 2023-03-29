using bookstore.Areas.Admin.Models;
using bookstore.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
       
        public AccountController(ApplicationDbContext db, IToastNotification toastNotification, RoleManager<IdentityRole> roleManager,UserManager<UserModel> userManager, SignInManager<UserModel> signInManager)
        {
            _db = db;
            _toastNotification = toastNotification;
            _roleManager = roleManager;
            _userManager = userManager;
            _SignInManager = signInManager;
        }
        [Route("index")]
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
                .ToList();
            ViewBag.book = bookmodel;

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
