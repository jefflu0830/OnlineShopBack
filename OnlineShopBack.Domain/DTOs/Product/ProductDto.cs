using Microsoft.AspNetCore.Http;
using System;
using System.Data;

namespace OnlineShopBack.Domain.DTOs.Product
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Num { get; set; }
        public int Category { get; set; }
        public int SubCategory { get; set; }
        public string SubCategoryName { get; set; }
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

        public static (bool, ProductDto) GetProductList(DataRow row)
        {
            ProductDto temp = new ProductDto();

            temp.Id = (int)row["f_id"];
            temp.Num = row["f_num"].ToString();
            temp.Category = (int)row["f_category"];
            temp.SubCategory = (int)row["f_subCategory"];
            temp.SubCategoryName = row["f_subCategoryName"].ToString();
            temp.Name = row["f_name"].ToString();
            temp.Price = (int)row["f_price"];
            temp.Status = (int)row["f_status"];
            temp.Stock = (int)row["f_stock"];
            temp.Popularity = (int)row["f_popularity"];
            temp.Content = row["f_content"].ToString();
            temp.ImgPath = row["f_img"].ToString();

            return (true, temp);
        }

    }
}
