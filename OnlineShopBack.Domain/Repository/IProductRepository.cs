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
    }
}
