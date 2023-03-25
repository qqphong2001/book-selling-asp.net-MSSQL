using bookstore.Areas.Admin.Models;
using bookstore.DbContext;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace bookstore.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("/admin/shippingMethod")]

    public class ShippingMethodController : Controller
    {
        readonly private ApplicationDbContext _db;
        readonly private IToastNotification _toastNotification;

        public ShippingMethodController(ApplicationDbContext db, IToastNotification toastNotification)
        {
            _db = db;
            _toastNotification = toastNotification;

        }

        public IActionResult Index()
        {
            IEnumerable<ShippingMethodModel> shippingMethods = _db.ShippingMethods.ToList();



            return View(shippingMethods);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> create(ShippingMethodModel obj)
        {

            await _db.ShippingMethods.AddAsync(obj);

            await _db.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage("tạo phương thức giao hàng thành công");
            return RedirectToAction("index");

        }

        [Route("delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {

            var shippingMethods = await _db.ShippingMethods.FindAsync(id);

            if (shippingMethods == null)
            {
                _toastNotification.AddErrorToastMessage("Không tìm thấy phương thức giao hàng");

                return RedirectToAction("index");
            }

            _db.ShippingMethods.Remove(shippingMethods);

            await _db.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage("Xóa phương thức giao hàng thành công");

            return RedirectToAction("index");

        }

        [Route("edit/{id}")]
        public IActionResult edit(int id)
        {
            var shipping = _db.ShippingMethods.Find(id);

            return View(shipping);
        }

        [Route("editpost")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> editpost([Bind("Id,Name,price")] ShippingMethodModel obj)
        {
            var shipping = await _db.ShippingMethods.FindAsync(obj.Id);
            if (shipping == null)
            {
                _toastNotification.AddErrorToastMessage("không thể sửa phương thức giao hàng");
                return RedirectToAction("index");
            }

            shipping.price = obj.price;
            shipping.Name = obj.Name;
            await _db.SaveChangesAsync(); 

            _toastNotification.AddSuccessToastMessage("chỉnh sửa thành công");
            return RedirectToAction("index");
        }
    }
}
