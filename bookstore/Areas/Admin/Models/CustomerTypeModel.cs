using System.ComponentModel.DataAnnotations;

namespace bookstore.Areas.Admin.Models
{
    public class CustomerTypeModel
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
