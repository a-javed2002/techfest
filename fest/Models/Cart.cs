using System;
using System.Collections.Generic;

namespace fest.Models
{
    public partial class Cart
    {
       

        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int? Price { get; set; }
        public string? ProductImage { get; set; }

        public int? Quantity { get; set; }

    }
}
