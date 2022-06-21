using Microsoft.AspNetCore.Http;
using System;

namespace OnlineShopBack.Domain.DTOs.Product
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Num { get; set; }
        public int Category { get; set; }
        public int SubCategory { get; set; }
        public string Name { get; set; } 
        public string ImgPath { get; set; }
        public IFormFile ImgFiles { get; set; }
        public int Price { get; set; }
        public int Status { get; set; }
        public string Content { get; set; } = string.Empty;
        public int Stock { get; set; }
        public int Popularity { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}
