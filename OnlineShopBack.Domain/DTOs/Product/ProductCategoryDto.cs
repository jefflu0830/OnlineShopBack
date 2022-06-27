using System.Data;

namespace OnlineShopBack.Domain.DTOs.Product
{
    public class ProductCategoryDto
    {
        public int CategoryNum { get; set; }
        public int SubCategoryNum { get; set; }
        public string SubCategoryName { get; set; }

        public static (bool, ProductCategoryDto) GetCategoryList(DataRow row)
        {
            ProductCategoryDto temp = new ProductCategoryDto();

            temp.CategoryNum = (int)row["f_categoryNum"];
            temp.SubCategoryNum= (int)row["f_subCategoryNum"];
            temp.SubCategoryName = row["f_subCategoryName"].ToString();


            return (true, temp);
        }
    }
}
