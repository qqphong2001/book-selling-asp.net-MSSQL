

using System.ComponentModel.DataAnnotations;

namespace bookstore.Areas.Admin.Models
{
    public class PublisherModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
