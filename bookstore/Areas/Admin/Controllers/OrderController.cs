using bookstore.Areas.Admin.Models;
using bookstore.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Stripe;

namespace bookstore.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("/admin/order")]

    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IToastNotification _toastMessage;
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;
        public OrderController(ApplicationDbContext db , IToastNotification toastNotification, UserManager<UserModel> userManager, SignInManager<UserModel> signInManager)
        {
            _db = db;
            _toastMessage = toastNotification;
            _userManager = userManager;
            _signInManager = signInManager;
            
        }

        [Route("index")]
        public IActionResult Index()
        {


            var result = _db.Orders.Join(
                _db.orderStatuses,
                Orders => Orders.status,
                orderStatuses => orderStatuses.Id,
                (Orders,orderStatuses) => new {orders = Orders , orderStatuses = orderStatuses }
                ).Join(
                _db.ShippingMethods,
                ordersOrStatus => ordersOrStatus.orders.shippingMethod_id,
                shippingMethod => shippingMethod.Id,
                (ordersOrStatus, shippingMethod) => new { orders = ordersOrStatus.orders , status = ordersOrStatus.orderStatuses , shipping = shippingMethod  }
                ).Join(
                _db.PaymentMethods,
                ordersOrStatusPay => ordersOrStatusPay.orders.paymentMethod_id,
                paymentMethod => paymentMethod.Id,
                (ordersOrStatusPay, paymentMethod)=> new {orders = ordersOrStatusPay.orders ,status = ordersOrStatusPay.status , payment = paymentMethod }
                ).Join(
                _db.Customers,
                ordercustomer => ordercustomer.orders.customer_id,
                customer => customer.Id,
                (ordercustomer, customer) => new {orders = ordercustomer.orders ,status = ordercustomer.status,payment = ordercustomer.payment ,customer = customer }
                ).Join(
                _db.customerAddresses,
                orderaddress => orderaddress.orders.customerAddress_id,
                address => address.Id,
                (orderaddress,address) => new { orders = orderaddress.orders , status = orderaddress.status , payment = orderaddress.payment,customer = orderaddress.customer ,address = address}
                ).
                Join(
                _userManager.Users,
                ordercustomer => ordercustomer.customer.account_id,
                user => user.Id,
                (ordercustomer, user) => new { orders = ordercustomer.orders, status = ordercustomer.status, payment = ordercustomer.payment, customer = ordercustomer.customer, address = ordercustomer.address , users = user }
                )
                .ToList();

            ViewBag.order = result;

            return View();
        }
    }
}
