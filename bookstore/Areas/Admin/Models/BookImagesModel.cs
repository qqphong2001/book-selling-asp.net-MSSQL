using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace bookstore.Areas.Admin.Models
{
    public class BookImagesModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [Required]
        public string image { get; set; }

        [Required]
        public int book_id { get; set; }
    }
}
