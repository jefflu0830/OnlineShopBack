using Microsoft.AspNetCore.Http;
using OnlineShopBack.Domain.DTOs.Product;
using System.Data;

namespace OnlineShopBack.Domain.Repository
{
    public interface IProductRepository
    {
        /*---------------商品相關---------------*/
        public DataTable GetProduct();//取得商品List

        public int AddProduct(ProductDto dto, IFormFileCollection files);//新增商品 

        public int UpdateProduct(ProductDto dto, IFormFileCollection files);//更新商品

        public int DelProduct(int ProductId, string ProductNum, string ImgName);//刪除商品

        /*---------------類別相關---------------*/

        public DataTable GetCategory(); //取得類別

        public int AddCategory(ProductCategoryDto value); //新增類別 

        public int UpdateCategory(int Num, int SubNum, ProductCategoryDto value);//更新類別

        public int DelCategory(int Num, int SubNum);//刪除類別
    }
}
