using bookstore.Areas.Admin.Models;
using bookstore.DbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace bookstore.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("/admin/author")]
    [Authorize(Roles = Role.Role_Admin + "," + Role.Role_Employee)]


    public class AuthorController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IToastNotification _toastNotification;
        public AuthorController(ApplicationDbContext db, IToastNotification toastNotification)
        {
            _db = db;
            _toastNotification = toastNotification; 
        }

        [Route("")]
        public IActionResult Index ()
        {
            IEnumerable<AuthorModel> objAuthorList = _db.Authors;

            return View(objAuthorList);
        }
 

        [Route("add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>  Create(AuthorModel obj)
        {
            if (obj == null)
            {
                _toastNotification.AddErrorToastMessage("Không thể thêm tác giả");

                return RedirectToAction("Index");

            }

            await _db.Authors.AddAsync(obj);
            await   _db.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Thêm tác giả thành công");

            return RedirectToAction("Index");


        }
        [Route("Detele/{id}")]
        public async Task<IActionResult> Detele(int? id)
        {

             var  author = await _db.Authors.FindAsync(id);
            if (author == null)
            {
                _toastNotification.AddErrorToastMessage("không thể xóa tác giả");
                return RedirectToAction("Index");

            }

            _db.Authors.Remove(author);
            await _db.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Xóa tác giả thành công");
            return RedirectToAction("Index");
        }

        [Route("Edit/{id}")]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var Author = _db.Authors.Find(id);
            if (Author == null)
            {
                _toastNotification.AddErrorToastMessage("không tìm thấy tác giả");

                return RedirectToAction("Index");

            }

            return View(Author);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("EditPost")]
        public async Task<IActionResult> EditPost([Bind("Id, Name, Description")] AuthorModel obj)
        {
            var author = await _db.Authors.FindAsync(obj.Id);
            if (author == null)
            {
                _toastNotification.AddErrorToastMessage("không tìm thấy tác giả");

                return RedirectToAction("Index");
            }

            author.Description = obj.Description;
            author.Name = obj.Name;

            await _db.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Chỉnh sửa tác giả thành công");

            return RedirectToAction("Index");


        }




    }
}
