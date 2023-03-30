namespace bookstore.Areas.Admin.Models
{
    public class Promotion
    {

        public int Id { get; set; } 

        public string code { get; set; }

        public int quantity { get; set; }

        public DateTime expireDate { get; set; }

        public int customer_id { get; set; }
    }
}
