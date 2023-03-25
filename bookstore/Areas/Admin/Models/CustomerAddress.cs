namespace bookstore.Areas.Admin.Models
{
    public class CustomerAddress
    {
        public int Id { get; set; }
        public string address { get; set; }   
        public int customer_id { get; set; }
    }
}
