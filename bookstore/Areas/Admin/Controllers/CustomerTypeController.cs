using bookstore.Areas.Admin.Models;
using bookstore.DbContext;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace bookstore.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("/admin/CustomerType")]

    public class CustomerTypeController : Controller
    {
        readonly private ApplicationDbContext _db;
        readonly private IToastNotification _toastNotification;

        public CustomerTypeController(ApplicationDbContext db, IToastNotification toastNotification)
        {
            _db = db;
            _toastNotification = toastNotification;

        }

        public IActionResult Index()
        {
            IEnumerable<CustomerTypeModel> customerTypes = _db.CustomerTypes.ToList();



            return View(customerTypes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> create(CustomerTypeModel obj)
        {

            await _db.CustomerTypes.AddAsync(obj);

            await _db.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage("tạo bậc thành viên thành công");
            return RedirectToAction("index");

        }

        [Route("delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {

            var customerType = await _db.CustomerTypes.FindAsync(id);

            if (customerType == null)
            {
                _toastNotification.AddErrorToastMessage("Không tìm thấy bậc hội viên");

                return RedirectToAction("index");
            }

            _db.CustomerTypes.Remove(customerType);

            await _db.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage("Xóa bậc hội viên thành công");

            return RedirectToAction("index");

        }

        [Route("edit/{id}")]
        public IActionResult edit(int id)
        {
            var customerType = _db.CustomerTypes.Find(id);

            return View(customerType);
        }

        [Route("editpost")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> editpost([Bind("Id,Name")] CustomerTypeModel obj)
        {
            var customerType = await _db.CustomerTypes.FindAsync(obj.Id);
            if (customerType == null)
            {
                _toastNotification.AddErrorToastMessage("không thể sửa bậc hội viên");
                return RedirectToAction("index");
            }


            customerType.Name = obj.Name;
            await _db.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage("chỉnh sửa bậc hội viên thành công");
            return RedirectToAction("index");
        }
    }
}
