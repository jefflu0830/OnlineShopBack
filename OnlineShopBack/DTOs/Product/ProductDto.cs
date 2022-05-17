using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopBack.Controllers
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Num { get; set; }
        public int Category { get; set; }
        public int SubCategory { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }
        public int Price { get; set; }
        public int Status { get; set; }
        public string Content { get; set; }
        public int Stock { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}
