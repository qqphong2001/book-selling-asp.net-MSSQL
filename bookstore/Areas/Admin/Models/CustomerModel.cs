
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace bookstore.Areas.Admin.Models
{
    public class CustomerModel
    {
        [Key]
        public int Id { get; set; }
        public string firstName { get; set; }

        public string lastName { get; set; }

    
        public int gender { get; set; }
      

        public DateTime dob { get; set; }
        public string phoneNumber { get; set; }
            
        public DateTime createdAt { get; set; } = DateTime.Now;
     

        public int point { get; set; }
      
        public string avatar { get; set; }

        public string account_id { get; set; }
       

        public int customerType_id { get; set; }







    }
}
