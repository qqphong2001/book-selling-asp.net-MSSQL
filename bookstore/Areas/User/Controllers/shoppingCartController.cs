
using bookstore.DbContext;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using bookstore.Areas.User.Service;
using bookstore.Areas.User.Models;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using bookstore.Areas.Admin.Models;
using Microsoft.AspNetCore.Identity;

namespace bookstore.Areas.User.Controllers
{
    [Area("user")]
    public class shoppingCartController : Controller
    {
        private readonly CartSevice _cartSevice;
        private readonly IToastNotification _toastNotification;
        private readonly ApplicationDbContext _db;
        private readonly SignInManager<bookstore.Areas.Admin.Models.UserModel> _SignInManager;
        private readonly UserManager<bookstore.Areas.Admin.Models.UserModel> _UserManager;
        public shoppingCartController(ApplicationDbContext db, IToastNotification toastNotification, CartSevice cartSevice, SignInManager<bookstore.Areas.Admin.Models.UserModel> SignInManager, UserManager<bookstore.Areas.Admin.Models.UserModel> UserManager)
        {
            _db = db;
            _toastNotification = toastNotification;
            _cartSevice = cartSevice;
            _SignInManager = SignInManager;
            _UserManager = UserManager;
        }

        [Route("/cart")]
        public IActionResult Index()
        {
            ViewData["title"] = "Trang giỏ hàng";



            if (_SignInManager.IsSignedIn(User))
            {
                var addresss = _db.Customers.Join(
                    _db.customerAddresses,
                    customers => customers.Id,
                    address => address.customer_id,
                    (customers, address) => new { customers = customers, address = address }
                    ).Where(x => x.customers.account_id == _UserManager.GetUserId(User)).ToList();

                ViewBag.address = addresss;

                var customer  = _db.Customers.Where(x => x.account_id == _UserManager.GetUserId(User)).FirstOrDefault();
                ViewBag.customer = customer;    
            }
            else
            {
                ViewBag.address = null;
                ViewBag.customer = null;
            }






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
                cartitem.quantity += Int32.Parse(Request.Form["quantity"]);
            }
            else
			{
				//  Thêm mới
                cart.Add(new CartItem() { quantity = Int32.Parse(Request.Form["quantity"]) , product = product });
            }

            // Lưu cart vào Session
           _cartSevice.SaveCartSession(cart);
            _toastNotification.AddSuccessToastMessage("Thêm sản phẩm vào giỏ hàng thành công");

            return Redirect(Request.Headers["Referer"].ToString());
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


        public async Task<IActionResult> checkout([FromRoute] int productid)
        {
            var cart = _cartSevice.GetCartItems();

            var cartitem = cart.Find(p => p.product.Id == productid);

          


            return Ok(cart);    

        }

      






    }
}
