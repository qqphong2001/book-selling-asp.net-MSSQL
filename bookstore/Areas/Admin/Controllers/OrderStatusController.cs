using bookstore.Areas.Admin.Models;
using bookstore.DbContext;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace bookstore.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("/admin/orderstatus")]
    public class OrderStatusController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IToastNotification _toastNotification;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public OrderStatusController(ApplicationDbContext db, IToastNotification toastNotification, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _toastNotification = toastNotification;
            _webHostEnvironment = webHostEnvironment;

        }

        public IActionResult Index()
        {
            IEnumerable<OrderStatus> order = _db.orderStatuses;

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add")]

        public async Task<IActionResult> create(OrderStatus obj)
        {

           

            await _db.orderStatuses.AddAsync(obj);
            await _db.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Thêm tình trạng đơn hàng");
            return RedirectToAction("Index");

        }

        [Route("delete/{id}")]

        public async Task<IActionResult> delete(int? id)
        {
            var status = await _db.orderStatuses.FindAsync(id);
            if (status == null)
            {
                _toastNotification.AddErrorToastMessage("Xóa tình trạng đơn hàng thất bại");

                return RedirectToAction("Index");

            }

            _db.orderStatuses.Remove(status);

            await _db.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Xóa tình trạng đơn hàng thành công");

            return RedirectToAction("Index");



        }

        [Route("edit/{id}")]

        public IActionResult edit(int? id)
        {
            var status = _db.orderStatuses.Find(id);


            if (status == null)
            {
                _toastNotification.AddErrorToastMessage("Sản phẩm không tồn tại");

                return RedirectToAction("Index");
            }

            return View(status);

        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("editpost")]
        public async Task<IActionResult> editpost([Bind("Id,Name")] OrderStatus obj)
        {
            var status = await _db.orderStatuses.FindAsync(obj.Id);

            if (status == null)
            {
                _toastNotification.AddErrorToastMessage("không tìm thấy tình trạng đơn hàng");
                return RedirectToAction("Index");
            }

            status.Name = obj.Name;
            await _db.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Chỉnh sửa thể loại thành công");

            return RedirectToAction("Index");




        }







    }
}
