

using System.ComponentModel.DataAnnotations;

namespace bookstore.Areas.Admin.Models
{
    public class ReviewModel
    {
        [Key]
        public int Id { get; set; }

        public string comment { get; set; }

        public int ranking { get; set; }

        public int customer_id { get; set; }

        public int book_id { get; set; }

        public DateTime DateTime { get; set; } = DateTime.Now;
    }
}
