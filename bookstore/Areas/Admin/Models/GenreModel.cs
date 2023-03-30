
using System.ComponentModel.DataAnnotations;

namespace bookstore.Areas.Admin.Models
{
    public class GenreModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        
        public string image { get; set; }
    }
}
