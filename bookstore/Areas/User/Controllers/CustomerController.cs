using bookstore.DbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using bookstore.Areas.Admin.Models;
using Microsoft.AspNetCore.Hosting;
using System.Collections;
using Stripe;
using Microsoft.AspNetCore.Identity;

namespace bookstore.Areas.User.Controllers
{
    [Authorize]
    [Area("user")]
    [Route("/customer")]
    public class CustomerController : Controller


    {

        private readonly ApplicationDbContext _db;
        private readonly IToastNotification _toastNotification;
        private readonly IWebHostEnvironment _webHostEnvironment;
        
        public CustomerController(IToastNotification toastNotification, ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _toastNotification = toastNotification;
            _db = db;
            _webHostEnvironment = webHostEnvironment;

        }

        [Route("/index/customer/{customer_id}")]
        public async Task<IActionResult> Index(string? customer_id)
        {


            var customer = await _db.Customers.Where(x => x.account_id == customer_id).FirstOrDefaultAsync();



            if (customer == null)
            {
                _toastNotification.AddErrorToastMessage("bạn chưa đăng nhập");
                return Redirect(Request.Headers["Referer"].ToString());
            }

            ViewBag.customer = customer;
            ViewBag.address2 = _db.customerAddresses.Where(x => x.customer_id == customer.Id).Skip(1).FirstOrDefault();
            ViewBag.address = _db.customerAddresses.Where(x => x.customer_id == customer.Id).FirstOrDefault();

            return View();

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("saveInfo")]

        public async Task<IActionResult> saveInfo(IFormFile? avatar)
        {
            var customer = await _db.Customers.FindAsync(int.Parse(Request.Form["Id"]));

            customer.phoneNumber = Request.Form["phoneNumber"];
            customer.lastName = Request.Form["lastName"];
            customer.firstName = Request.Form["firstName"];
            customer.dob = DateTime.Parse(Request.Form["dob"]);
            customer.gender = int.Parse(Request.Form["gender"]);


            if (avatar != null)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                string fileName = Guid.NewGuid().ToString();
                string uploads = Path.Combine(wwwRootPath, @"images\avatar");
                var extension = Path.GetExtension(avatar.FileName);

                await using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    await avatar.CopyToAsync(fileStreams);
                }
                customer.avatar = @"\images\avatar\" + fileName + extension;

            }

            _db.Customers.Update(customer);
            await _db.SaveChangesAsync();

            if (_db.customerAddresses.Where(x => x.customer_id == customer.Id).Any())
            {
                if (Request.Form["address1"].Any())
                {
                    
                    if(_db.customerAddresses.Where(x => x.customer_id == customer.Id).FirstOrDefault() != null)
                    {

                    }
                    else
                    {
                        var addressAdd = new CustomerAddress
                        {
                            customer_id = customer.Id,
                            address = Request.Form["address1"]
                        };

                        _db.customerAddresses.Add(addressAdd);
                        await _db.SaveChangesAsync();


                    }

                }

                if (Request.Form["address2"].Any())
                {

                    if (_db.customerAddresses.Where(x => x.customer_id == customer.Id).Skip(1).FirstOrDefault() != null)
                    {

                    }
                    else
                    {
                        var addressAdd = new CustomerAddress
                        {
                            customer_id = customer.Id,
                            address = Request.Form["address2"]
                        };

                        _db.customerAddresses.Add(addressAdd);
                        await _db.SaveChangesAsync();


                    }

                }



                var address = _db.customerAddresses.Where(x => x.customer_id == customer.Id).ToList();
                var count = 0;
                foreach (var item in address)
                {
                    count++;
                    item.address = Request.Form["address" + count];
                }
                _db.customerAddresses.UpdateRange(address);
                await _db.SaveChangesAsync();

            }
            else
            {

                if (Request.Form["address1"].Any())
                {
                    var address1 = new CustomerAddress
                    {
                        address = Request.Form["address1"],
                        customer_id = customer.Id

                    };
                    _db.customerAddresses.Add(address1);
                    await _db.SaveChangesAsync();

                }
                else if (Request.Form["address2"].Any())
                {
                    var address2 = new CustomerAddress
                    {
                        address = Request.Form["address2"],
                        customer_id = customer.Id

                    };
                    _db.customerAddresses.Add(address2);
                    await _db.SaveChangesAsync();

                }




            }
            _toastNotification.AddSuccessToastMessage("Chỉnh sửa thông tin cá nhân thành công");
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [Route("/order/customer/{order}")]
        public IActionResult order(string? order)
        {
            var customer = _db.Customers.Join(
                _db.Orders,
                customer => customer.Id,
                order => order.customer_id,
                (customer,order) => new {customer = customer , order = order}
                ).Join(
                _db.ShippingMethods,
                customershipping => customershipping.order.shippingMethod_id,
                shipping => shipping.Id,
                (customershipping,shipping) => new { customer = customershipping.customer, order = customershipping.order, shipping = shipping }
                ).Join(
                _db.customerAddresses,
                customeraddress =>customeraddress.order.customerAddress_id,
                address => address.Id,
                (customeraddress,address) => new { customer = customeraddress.customer , order = customeraddress.order, address = address,shipping = customeraddress.shipping }

                )
                .Join(
                _db.orderStatuses,
                customerpayment => customerpayment.order.status,
                payment => payment.Id,
                (customerpayment, payment) => new { customer = customerpayment.customer, order = customerpayment.order, address = customerpayment.address, shipping = customerpayment.shipping, payment = payment }
                )

                .Where(x => x.customer.account_id == order).ToList();


          
            ViewBag.order = customer;




            return View();

        }
        [Route("/detail/customer/{id}")]
        public IActionResult Detail(int? id)
        {
            var result = _db.Orders.Join(
               _db.orderStatuses,
               Orders => Orders.status,
               orderStatuses => orderStatuses.Id,
               (Orders, orderStatuses) => new { orders = Orders, orderStatuses = orderStatuses }
               ).Join(
               _db.ShippingMethods,
               ordersOrStatus => ordersOrStatus.orders.shippingMethod_id,
               shippingMethod => shippingMethod.Id,
               (ordersOrStatus, shippingMethod) => new { orders = ordersOrStatus.orders, status = ordersOrStatus.orderStatuses, shipping = shippingMethod }
               ).Join(
               _db.PaymentMethods,
               ordersOrStatusPay => ordersOrStatusPay.orders.paymentMethod_id,
               paymentMethod => paymentMethod.Id,
               (ordersOrStatusPay, paymentMethod) => new { orders = ordersOrStatusPay.orders, status = ordersOrStatusPay.status, payment = paymentMethod, shipping = ordersOrStatusPay.shipping }
               ).Join(
               _db.Customers,
               ordercustomer => ordercustomer.orders.customer_id,
               customer => customer.Id,
               (ordercustomer, customer) => new { orders = ordercustomer.orders, status = ordercustomer.status, payment = ordercustomer.payment, customer = customer, shipping = ordercustomer.shipping }
               ).Join(
               _db.customerAddresses,
               orderaddress => orderaddress.orders.customerAddress_id,
               address => address.Id,
               (orderaddress, address) => new { orders = orderaddress.orders, status = orderaddress.status, payment = orderaddress.payment, customer = orderaddress.customer, address = address, shipping = orderaddress.shipping }
            )
               .Where(x => x.orders.Id == id).FirstOrDefault();

            ViewBag.order = result;

            ViewBag.cart = _db.OrderDetails.Where(x => x.order_id == id).
                Join(
                _db.Books,
                cart => cart.book_id,
                book => book.Id,
                (cart, book) => new { cart = cart, book = book }
                )
                .
                ToList();



            return View("orderdetail","customer");
        }


        [Route("review")]
        public IActionResult review([FromForm] int bookid, [FromForm] int customerid, [FromForm] int ratingValue , [FromForm] string description)
        {


           

            var review = new ReviewModel()
            {
                book_id = bookid,
                comment = description,
                customer_id = customerid,
                ranking = ratingValue,
            };

            _db.Reviews.Add(review);
            _db.SaveChanges();

            _toastNotification.AddSuccessToastMessage("bạn đã đánh giá thành công");
            
            return Redirect(Request.Headers["Referer"].ToString());
        }

    }
}







