
using bookstore.DbContext;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using bookstore.Areas.User.Service;
using bookstore.Areas.User.Models;
using Microsoft.EntityFrameworkCore;

namespace bookstore.Areas.User.Controllers
{
    [Area("user")]
    public class shoppingCartController : Controller
    {
        private readonly CartSevice _cartSevice;
        private readonly IToastNotification _toastNotification;
        private readonly ApplicationDbContext _db;
        public shoppingCartController(ApplicationDbContext db, IToastNotification toastNotification, CartSevice cartSevice)
        {
            _db = db;
            _toastNotification = toastNotification;
            _cartSevice = cartSevice;
        }

        [Route("/cart")]
        public IActionResult Index()
        {
            return View(_cartSevice.GetCartItems());
        }

        /// Thêm sản phẩm vào cart
        [Route("addcart/{productid:int}", Name = "addcart")]
        public IActionResult AddToCart([FromRoute] int productid)
        {

            var product = _db.Books
                .Where(p => p.Id == productid)
                .FirstOrDefault();
            if (product == null)
                return NotFound("Không có sản phẩm");

            // Xử lý đưa vào Cart ...
            var cart = _cartSevice.GetCartItems();
            var cartitem = cart.Find(p => p.product.Id == productid);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cartitem.quantity++;
            }
            else
            {
                //  Thêm mới
                cart.Add(new CartItem() { quantity = 1, product = product });
            }

            // Lưu cart vào Session
           _cartSevice.SaveCartSession(cart);
            // Chuyển đến trang hiện thị Cart
            return RedirectToAction("index");
        }


        [Route("/updatecart", Name = "updatecart")]
        [HttpPost]
        public IActionResult UpdateCart([FromForm] int productid, [FromForm] int quantity)
        {
            // Cập nhật Cart thay đổi số lượng quantity ...
            var cart = _cartSevice.GetCartItems();
            var cartitem = cart.Find(p => p.product.Id == productid);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cartitem.quantity = quantity;
            }
            _cartSevice.SaveCartSession(cart);
            // Trả về mã thành công (không có nội dung gì - chỉ để Ajax gọi)
            return Ok();
        }


        [Route("/removecart/{productid:int}", Name = "removecart")]
        public IActionResult RemoveCart([FromRoute] int productid)
        {
            var cart = _cartSevice.GetCartItems();
            var cartitem = cart.Find(p => p.product.Id == productid);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cart.Remove(cartitem);
            }

          _cartSevice.SaveCartSession(cart);
            return RedirectToAction("index");
        }





    }
}
