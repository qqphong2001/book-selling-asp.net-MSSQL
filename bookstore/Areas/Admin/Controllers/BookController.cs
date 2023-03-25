using bookstore.Areas.Admin.Models;
using bookstore.DbContext;


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Collections;

namespace bookstore.Areas.Admin.Controllers
{
    //[authorize(roles = role.role_admin)]
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

            IEnumerable<BookImagesModel> obj = _db.BookImages.Where(i => i.book_id == id).ToList();
       
            ViewBag.BookImages = obj;


            return View(book);

        }

        [Route("editpost")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> editpost([Bind("Id,title,isbn,description,numPages,layout,publishDate,weight,translatorName,hSize,wSize,unitPrice,unitStock,discount,cover,publisher_id,author_id,genre_id")]BookModel books ,[Bind("image")] BookImagesModel bookimages,IFormFile[]? FileUploads, IFormFile imageFile)
        {
       
            string wwwRootPath = _webHostEnvironment.WebRootPath;

            var book = await _db.Books.FindAsync(books.Id);
            if (book == null)
            {
                _toastNotification.AddErrorToastMessage("Không tìm thấy sản phẩm");
                return RedirectToAction("Index");
            }

            if (imageFile != null)
            {
                string fileName = Guid.NewGuid().ToString();
                string uploads = Path.Combine(wwwRootPath, @"images\products");
                var extension = Path.GetExtension(imageFile.FileName);

                await using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStreams);
                }
                book.cover = @"\images\products\" + fileName + extension;
            }
            book.title = books.title;
            book.description = books.description;
            book.isbn = books.isbn;
            book.discount = books.discount; 
            book.weight = books.weight;
            book.unitStock = books.unitStock;
            book.unitPrice = books.unitPrice;
            book.numPages = books.numPages; 
            book.genre_id = books.genre_id; 
            book.publisher_id = books.publisher_id; 
            book.hSize = books.hSize;
            book.author_id = books.author_id;
            book.publishDate = books.publishDate;   
            book.translatorName = books.translatorName;
            book.wSize = books.wSize;
            book.layout = books.layout;

            await _db.SaveChangesAsync();


            if (FileUploads != null)
            {
                var count = 0;
                ArrayList arrList = new ArrayList();

                foreach (var FileUpload in FileUploads)
                {
                    string fileName = Guid.NewGuid().ToString();
                    string uploads = Path.Combine(wwwRootPath, @"images\products\thumbnail");
                    var extension = Path.GetExtension(FileUpload.FileName);

                    await using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        await FileUpload.CopyToAsync(fileStream);
                        arrList.Add(@"\images\products\thumbnail\" + fileName + extension);
                    }
                }

                foreach (var file in arrList)
                {
                  
                    
                   var  bookimage =   await _db.BookImages.FindAsync(Int32.Parse(Request.Form["book" + count++]));
                    bookimage.image = file.ToString();
                    await _db.SaveChangesAsync();


                }

               

            }
            _toastNotification.AddSuccessToastMessage("Chỉnh sửa sản phẩm thành công");

            return RedirectToAction("Index");   



        }
    }
}
