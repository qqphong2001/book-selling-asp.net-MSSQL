using System.ComponentModel.DataAnnotations;

namespace bookstore.Areas.Admin.Models
{
    public class PaymentMethodModel
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string picture { get; set; }

        public int status { get; set; }
       



    }
}
