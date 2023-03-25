﻿using System.ComponentModel.DataAnnotations;

namespace bookstore.Areas.Admin.Models
{
    public class OrderStatus
    {

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
