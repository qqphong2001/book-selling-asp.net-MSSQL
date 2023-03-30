

using System.ComponentModel.DataAnnotations;

namespace bookstore.Areas.Admin.Models
{
    public class ShippingMethodModel
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public float price { get; set; }
        public int status { get; set; }



    }
}
