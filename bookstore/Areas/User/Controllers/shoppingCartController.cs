
using bookstore.DbContext;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using bookstore.Areas.User.Service;
using bookstore.Areas.User.Models;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using bookstore.Areas.Admin.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;

using static System.Runtime.CompilerServices.RuntimeHelpers;
using System.Security.Policy;
using NuGet.Common;
using Org.BouncyCastle.Asn1.X9;
using Stripe.Checkout;

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

                var customer = _db.Customers.Where(x => x.account_id == _UserManager.GetUserId(User)).FirstOrDefault();
                ViewBag.customer = customer;



            }
            else
            {
                ViewBag.address = null;
                ViewBag.customer = null;
            }


            var shippingMethod = _db.ShippingMethods.ToList();
            var paymentMethod = _db.PaymentMethods.ToList();

            ViewBag.paymentMethod = paymentMethod;
            ViewBag.shippingMethod = shippingMethod;



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
                cart.Add(new CartItem() { quantity = Int32.Parse(Request.Form["quantity"]), product = product });
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
                cartitem.quantity = quantity++;
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


        [Route("checkout")]
        public async Task<IActionResult> checkout([FromRoute] int productid)
        {


            var cart = _cartSevice.GetCartItems();
            float totalMax = 0;


            foreach (var item in cart)
            {
                float total = item.quantity * item.product.unitPrice;
                totalMax += total;
            }

            int number = 999999999;

            Random random = new Random();
            var randomNumber = random.Next(number).ToString();


            var orderNumbers = _db.Orders.Select(x => x.orderNumber).ToList();

            if (orderNumbers.Contains(randomNumber))
            {
                randomNumber = random.Next(number).ToString();

            }



            var order = new OrderModel
            {
                orderNumber = $"#{randomNumber}",
                orderDate = DateTime.Now,
                total = totalMax,
                description = Request.Form["description"],
                customer_id = int.Parse(Request.Form["customer_id"]),
                customerAddress_id = int.Parse(Request.Form["address"]),
                status = 0,
                paymentMethod_id = int.Parse(Request.Form["payment"]),
                shippingMethod_id = int.Parse(Request.Form["shipping"])


            };
            var order_id = await _db.Orders.AddAsync(order);
            await _db.SaveChangesAsync();

            foreach (var item in cart)
            {
                var orderdetail = new OrderDetailModel
                {
                    book_id = item.product.Id,
                    quantity = item.quantity,
                    order_id = order_id.Entity.Id,
                };
                await _db.OrderDetails.AddAsync(orderdetail);
                await _db.SaveChangesAsync();

            }


            if (Request.Form["payment"] == "6")
            {
                var domain = "https://localhost:44355/";
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string>
                    {
                        "card"
                    },
                    LineItems = new List<SessionLineItemOptions>()
                  ,
                    Mode = "payment",
                    SuccessUrl = domain + $"successCart/{order_id.Entity.Id}",
                    CancelUrl = domain,
                };
                long StripeTotalMax = 0;
                foreach (var item in cart)
                {
                    //var StripeTotal = item.quantity * item.product.unitPrice;


                    var newSessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)item.product.unitPrice,
                            Currency = "VND",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.product.title,
                            },
                        },
                        Quantity = item.quantity,
                    };
                    options.LineItems.Add(newSessionLineItem);


                }

                var service = new SessionService();
                Session session = service.Create(options);

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            return Redirect("/");

        }

        [Route("successCart/{id}")]
        public async Task<IActionResult> successCart(int? id)
        {

            var order = _db.Orders.Find(id);

            order.paymentDate = DateTime.Now;
            order.status = 1;

            _db.Orders.Update(order);

            _db.SaveChanges();


            return View();
        }









    }
}
