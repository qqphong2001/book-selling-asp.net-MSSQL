using System.ComponentModel.DataAnnotations;

namespace bookstore.Areas.Admin.Models
{
    public class OrderModel
    {
        [Key]
        public int Id { get; set; }

        public string orderNumber { get; set; }
        public DateTime orderDate { get; set; }
        public float total { get; set; }
        public float discount { get; set; }
        public DateTime  paymentDate  { get; set; }
        public int status { get; set; }
        public int shippingMethod_id { get; set; }
        public int paymentMethod_id { get; set; }
        public int customer_id { get; set; }
        public int customerAddress_id { get; set; }
        public int promotion_id { get; set; }
        public int emloyee_id { get; set; }



    }
}
