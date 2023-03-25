using bookstore.DbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using bookstore.Areas.Admin.Models;
using Microsoft.AspNetCore.Hosting;
using System.Collections;

namespace bookstore.Areas.User.Controllers
{
    [Authorize]
    [Area("user")]
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

        [Route("index/{customer_id}")]
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

                }
                else if (Request.Form["address2"].Any())
                {
                    var address2 = new CustomerAddress
                    {
                        address = Request.Form["address2"],
                        customer_id = customer.Id

                    };
                    _db.customerAddresses.Add(address2);

                }




            }
            _toastNotification.AddSuccessToastMessage("Chỉnh sửa thông tin cá nhân thành công");
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}







