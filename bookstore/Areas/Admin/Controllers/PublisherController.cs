using bookstore.Areas.Admin.Models;
using bookstore.DbContext;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Security.Policy;

namespace bookstore.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("/admin/publisher")]
    public class PublisherController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IToastNotification _toastNotification;
        public PublisherController(ApplicationDbContext db, IToastNotification toastNotification) { 
            _db = db;
            _toastNotification = toastNotification; 
        }

        [Route("")]
        public IActionResult Index()
        {
            IEnumerable<PublisherModel> publisherModels = _db.Publisher;

            return View(publisherModels);
        }

        [Route("add")]
        [ValidateAntiForgeryToken]
        [HttpPost]

        public async Task<IActionResult> Create(PublisherModel obj)
        {
            if (obj == null)
            {
                _toastNotification.AddErrorToastMessage("Không thể thêm nhà xuất bản");
                return RedirectToAction("Index");

            }

            await _db.Publisher.AddAsync(obj);
            await _db.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Thêm nhà xuất bản thành công");

            return RedirectToAction("Index");

        }

        [Route("delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            var publisher = await _db.Publisher.FindAsync(id);

            if (publisher == null)
            {
                _toastNotification.AddErrorToastMessage("Không thể xóa nhà xuất bản");
                return RedirectToAction("Index");
            }

             _db.Publisher.Remove(publisher);

            await _db.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Xóa nhà xuất bản thành công");

            return RedirectToAction("Index");

        }

        [Route("edit/{id}")]
        public IActionResult Edit(int? id)
        {
            var publisher = _db.Publisher.Find(id);

            if(publisher == null)
            {
                _toastNotification.AddErrorToastMessage("Không tìm thấy nhà xuất bản");

                return RedirectToAction("Index");
            }

            return View(publisher);



        }

        [Route("editpost")]
        [ValidateAntiForgeryToken][HttpPost]
        public async Task<IActionResult> editpost([Bind("Name,Id")] PublisherModel obj )
        {
            var publisher = await _db.Publisher.FindAsync(obj.Id);
            if (publisher == null)
            {
                _toastNotification.AddErrorToastMessage("Không tìm thấy nhà xuất bản");

                return RedirectToAction("Index");
            }

            publisher.Name = obj.Name;

            await _db.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Chỉnh sửa nhà xuất bản thành công");

            return RedirectToAction("Index");

        }



    }
}
