
using System.ComponentModel.DataAnnotations;

namespace bookstore.Areas.Admin.Models
{
    public class AuthorModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

    }
}
