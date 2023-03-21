using bookstore.Areas.Admin.Models;
using bookstore.DbContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using NToastNotify;
using System.Collections.Generic;

namespace bookstore.Areas.Admin.Controllers
{

    [Area("admin")]
    [Route("/admin/genre")]
    public class GenreController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IToastNotification _toastNotification;
        public GenreController(ApplicationDbContext db,IToastNotification toastNotification)
        {
            _db = db;
            _toastNotification = toastNotification;
            
        }

        public IActionResult Index()
        {
            IEnumerable<GenreModel> genreModels = _db.Genres;

            return View(genreModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add")]
        
        public async Task<IActionResult> create(GenreModel obj)
        {
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
                return RedirectToAction("Index");
                _toastNotification.AddErrorToastMessage("Xóa thể loại thất bại");

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
        public async Task<IActionResult> editpost([Bind("Id,Name")]GenreModel obj )
        {
            var genre = await _db.Genres.FindAsync(obj.Id);
            if (genre == null)
            {
                _toastNotification.AddErrorToastMessage("không tìm thấy sản phẩm");
                return RedirectToAction("Index");
            }

            genre.Name = obj.Name;
            await _db.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Chỉnh sửa thể loại thành công");

            return RedirectToAction("Index");




        }







    }
}
