using bookstore.Areas.Admin.Models;
using bookstore.DbContext;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace bookstore.Areas.Admin.Controllers
{   
    [Area("admin")]
    [Route("/admin/paymentmethod")]
    public class PaymentMethodController : Controller
    {
        readonly private ApplicationDbContext _db;
        readonly private IToastNotification _toastNotification;
        readonly private IWebHostEnvironment _webHostEnvironment;
        public PaymentMethodController(ApplicationDbContext db, IToastNotification toastNotification, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _toastNotification = toastNotification;
            _webHostEnvironment = webHostEnvironment;

        }

        [Route("index")]
        public IActionResult Index()
        {
            IEnumerable<PaymentMethodModel> payments = _db.PaymentMethods.ToList();



            return View(payments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> create(PaymentMethodModel obj, IFormFile? imageFile)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (imageFile != null)
            {
                string fileName = Guid.NewGuid().ToString();
                string uploads = Path.Combine(wwwRootPath, @"images\payment");
                var extension = Path.GetExtension(imageFile.FileName);

                await using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStreams);
                }
                obj.picture = @"\images\payment\" + fileName + extension;

            }
            await _db.PaymentMethods.AddAsync(obj);

            await _db.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage("tạo phương thức thanh toán thành công");
            return RedirectToAction("index");

        }

        [Route("delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {

            var payemnt = await _db.PaymentMethods.FindAsync(id);

            if (payemnt == null)
            {
                _toastNotification.AddErrorToastMessage("Không tìm thấy phương thức thanh toán");

                return RedirectToAction("index");
            }

            _db.PaymentMethods.Remove(payemnt);

            await _db.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage("Xóa phương thức thanht toán thành công");

            return RedirectToAction("index");

        }

        [Route("edit/{id}")]
        public IActionResult edit(int id)
        {
            var payment = _db.PaymentMethods.Find(id);

            return View(payment);
        }

        [Route("editpost")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> editpost([Bind("Id,Name")] ShippingMethodModel obj, IFormFile? imageFile)
        {
            var payment  = await _db.PaymentMethods.FindAsync(obj.Id);
            if (payment == null)
            {
                _toastNotification.AddErrorToastMessage("không thể sửa phương thức thanh toán");
                return RedirectToAction("index");
            
            }

            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (imageFile != null)
            {
                string fileName = Guid.NewGuid().ToString();
                string uploads = Path.Combine(wwwRootPath, @"images\payment");
                var extension = Path.GetExtension(imageFile.FileName);

                await using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStreams);
                }
                payment.picture = @"\images\payment\" + fileName + extension;

            }


            payment.Name = obj.Name;
            await _db.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage("chỉnh sửa thành công");
            return RedirectToAction("index");
        }
    }
}
