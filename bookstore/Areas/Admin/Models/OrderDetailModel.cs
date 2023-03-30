using System.ComponentModel.DataAnnotations;

namespace bookstore.Areas.Admin.Models
{
    public class OrderDetailModel
    {
        [Key]
        public int Id { get; set; }

        public int quantity { get; set; }
        public int order_id { get; set; }
        public int book_id { get; set; }
        public int customer_id { get; set; }


    }
}
