using bookstore.Areas.Admin.Models;

namespace bookstore.Areas.User.Models
{
    public class CartItem

    {
        public int quantity { get; set; }
        public BookModel product { get; set; }
    }
}
