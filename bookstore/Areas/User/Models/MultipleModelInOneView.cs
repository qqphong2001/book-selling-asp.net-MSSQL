

namespace bookstore.Areas.User.Models
{
    public class MultipleModelInOneView
    {
        public IEnumerable<bookstore.Areas.User.Models.CartItem> CartItems { get; set; }
        public IEnumerable<bookstore.Areas.Identity.Pages.Account.LoginModel> loginModels { get; set; }
    }
}
