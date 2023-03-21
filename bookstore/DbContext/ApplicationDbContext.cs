namespace bookstore.DbContext;

using bookstore.Areas.Admin.Models;
using Microsoft.EntityFrameworkCore;


public class ApplicationDbContext : DbContext
{

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base (options)
    {
        
    }
   

    public DbSet<AuthorModel> Authors { get; set; }
   public DbSet<PublisherModel> Publisher { get; set; }
    public DbSet<GenreModel> Genres { get; set; }

    public DbSet<BookModel> Books { get; set; }

    public DbSet<BookImagesModel> BookImages { get; set; }



}
