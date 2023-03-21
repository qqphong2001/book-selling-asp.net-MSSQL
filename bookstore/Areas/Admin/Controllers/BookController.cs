using bookstore.Areas.Admin.Models;
using bookstore.DbContext;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace bookstore.Areas.Admin.Controllers
{
    [Area("admin")]

    [Route("/admin/book")]
    public class BookController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly IToastNotification _toastNotification;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BookController(ApplicationDbContext db, IToastNotification toastNotification,IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _toastNotification = toastNotification;
            _webHostEnvironment = webHostEnvironment;
        }
        [Route("")]
        [Route("~/")]
        public IActionResult Index()
        {

            IEnumerable<BookModel> books = _db.Books;

            return View(books);
        }


        [Route("add")]
        [HttpPost]
        [ValidateAntiForgeryToken]



        public async Task<IActionResult> create(BookModel obj,IFormFile? imageFile,BookImagesModel imagebook ,IFormFile[]? FileUploads)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;     

                if (obj == null)
            {

                _toastNotification.AddErrorToastMessage("Thêm sách thất bại");

                return RedirectToAction("Index");

            }
            if(imageFile != null)
            {
                string fileName = Guid.NewGuid().ToString();
                string uploads = Path.Combine(wwwRootPath, @"images\products");
                var extension = Path.GetExtension(imageFile.FileName);

                await using(var fileStreams = new FileStream(Path.Combine(uploads,fileName + extension) , FileMode.Create))
                {
                   await  imageFile.CopyToAsync(fileStreams);
                }
                 obj.cover = @"\images\products\" + fileName + extension;
               
            }
            
                  
            var book =   await _db.Books.AddAsync(obj);

            await _db.SaveChangesAsync();

            if (FileUploads != null)
            {
              
                foreach (var FileUpload in FileUploads)
                {
                    string fileName = Guid.NewGuid().ToString();
                    string uploads = Path.Combine(wwwRootPath, @"images\products\thumbnail");
                    var extension = Path.GetExtension(FileUpload.FileName);

                    await using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        await FileUpload.CopyToAsync(fileStream);
                        imagebook.image = @"\images\products\thumbnail\" + fileName + extension;
                        imagebook.book_id = book.Entity.Id;
                        await _db.BookImages.AddAsync(imagebook);

                        imagebook.Id = 0;

                        await _db.SaveChangesAsync();

                    }
                }

            }


            _toastNotification.AddSuccessToastMessage("Thêm sản phẩm thành công");

            return RedirectToAction("index");




        }
        [Route("delete/{id}")]

        public async Task<IActionResult> Delete(int? id)
        {
            var book = await _db.Books.FindAsync(id);

            if (book == null)
            {
                _toastNotification.AddErrorToastMessage("Sản phẩm không tồn tại");
                return RedirectToAction("Index");

            }

            _db.Books.Remove(book);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");

        }


        [Route("edit/{id}")]

        public IActionResult Edit(int? id) { 
        
            var book = _db.Books.Find(id);
            if (book == null) {

                _toastNotification.AddErrorToastMessage("Không tìm thấy sách");

                return RedirectToAction("Index");
            }

            return View(book);

        }




    }
}
