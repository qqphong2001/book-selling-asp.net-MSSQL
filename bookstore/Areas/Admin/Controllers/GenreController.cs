using bookstore.Areas.Admin.Models;
using bookstore.DbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using NToastNotify;
using System.Collections.Generic;

namespace bookstore.Areas.Admin.Controllers
{

    [Area("admin")]
    [Route("/admin/genre")]
    [Authorize(Roles = Role.Role_Admin + "," + Role.Role_Employee)]

    public class GenreController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IToastNotification _toastNotification;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public GenreController(ApplicationDbContext db,IToastNotification toastNotification,IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _toastNotification = toastNotification;
            _webHostEnvironment = webHostEnvironment;
            
        }

        public IActionResult Index()
        {
            IEnumerable<GenreModel> genreModels = _db.Genres;

            return View(genreModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add")]
        
        public async Task<IActionResult> create(GenreModel obj,IFormFile? imageFile)
        {

            string wwwRootPath = _webHostEnvironment.WebRootPath;

            if (imageFile != null)
            {
                string fileName = Guid.NewGuid().ToString();
                string uploads = Path.Combine(wwwRootPath, @"images\genre");
                var extension = Path.GetExtension(imageFile.FileName);

                await using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStreams);
                }
                obj.image = @"\images\genre\" + fileName + extension;

            }

            await _db.Genres.AddAsync(obj);
            await _db.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Thêm thể loại thành công");
            return RedirectToAction("Index");

        }

        [Route("delete/{id}")]

        public async Task<IActionResult> delete(int? id)
        {
            var genre = await _db.Genres.FindAsync(id);
            if (genre == null)
            {
                _toastNotification.AddErrorToastMessage("Xóa thể loại thất bại");

                return RedirectToAction("Index");

            }

            _db.Genres.Remove(genre);

            await _db.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Xóa thể loại thành công");

            return RedirectToAction("Index");



        }

        [Route("edit/{id}")]

        public IActionResult edit(int? id)
        {
            var genre = _db.Genres.Find(id);  
            






            if (genre == null)
            {
                _toastNotification.AddErrorToastMessage("Sản phẩm không tồn tại");

                return RedirectToAction("Index");
            }

            return View(genre);

        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("editpost")]
        public async Task<IActionResult> editpost([Bind("Id,Name")]GenreModel obj, IFormFile? imageFile)
        {
            var genre = await _db.Genres.FindAsync(obj.Id);

            if (genre == null)
            {
                _toastNotification.AddErrorToastMessage("không tìm thấy sản phẩm");
                return RedirectToAction("Index");
            }

            if (imageFile != null)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString();
                string uploads = Path.Combine(wwwRootPath, @"images\genre");
                var extension = Path.GetExtension(imageFile.FileName);

                await using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStreams);
                }
                genre.image = @"\images\genre\" + fileName + extension;

            }
            genre.Name = obj.Name;
            await _db.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Chỉnh sửa thể loại thành công");

            return RedirectToAction("Index");




        }







    }
}
