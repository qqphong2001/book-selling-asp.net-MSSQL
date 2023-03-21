
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;

namespace bookstore.Areas.Admin.Models
{
    public class BookModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string isbn { get; set; }

        [Required]
        public string title { get; set; }

        public string description { get; set; }

        public string numPages { get; set;}

        public string layout { get; set;}

        public DateTime publishDate { get; set; } 

        public int view { get; set; }

        public float  weight { get; set; }

        public string translatorName { get; set; }

        public float  hSize { get; set; }

        public float wSize { get; set; }

        public decimal unitPrice { get; set; }

        public int unitStock { get; set; }

        public float ranking { get; set; }

        public float discount { get; set; }

        public string cover { get; set; }

        public int publisher_id { get; set; }
        public int author_id { get; set; }

        public int genre_id { get; set; }






    }
}
