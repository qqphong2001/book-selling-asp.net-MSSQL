namespace bookstore.DbContext;

using bookstore.Areas.Admin.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


public class ApplicationDbContext : IdentityDbContext<UserModel>
{

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base (options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            if (tableName.StartsWith("AspNet"))
            {
                entityType.SetTableName(tableName.Substring(6));
            }

        }
    }
   

    public DbSet<AuthorModel> Authors { get; set; }
   public DbSet<PublisherModel> Publisher { get; set; }
    public DbSet<GenreModel> Genres { get; set; }

    public DbSet<BookModel> Books { get; set; }

    public DbSet<BookImagesModel> BookImages { get; set; }



}
