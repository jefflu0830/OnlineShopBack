#region 功能與歷史修改描述
/*
    描述:商品系統相關
    日期:2022-05-09
*/
#endregion

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Domain.DTOs.Product;
using OnlineShopBack.Domain.Repository;
using OnlineShopBack.Domain.Tool;
using OnlineShopBack.Services;
using System;
using System.Data;
using System.Linq;
using System.Text.Json;

namespace OnlineShopBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    { 

        private readonly IProductRepository _accountService = null;
        public ProductController(IProductRepository accountService)
        {
            _accountService = accountService;
        }
        /*---------------商品相關---------------*/
        //取得商品List
        [HttpGet("GetProduct")]
        public string GetProduct()
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "未有使用權限";
            }

            DataTable dt = _accountService.GetProduct();

            ProductDto[] ProductList = dt.Rows.Cast<DataRow>()
                 .Select(row => ProductDto.GetProductList(row))
                 .Where(Tuple => Tuple.Item1 == true)
                 .Select(Tuple => Tuple.Item2)
                 .ToArray();

            string Result = JsonSerializer.Serialize(ProductList);//序列化回傳  回傳型態 string
            return Result;
        }

        //新增商品 
        [HttpPost("AddProduct")]
        public string AddProduct()
        {
            //其他文字資訊
            OnlineShopBack.Domain.DTOs.Product.ProductDto dto = JsonSerializer.Deserialize<OnlineShopBack.Domain.DTOs.Product.ProductDto>(Request.Form["AddProductFrom"]);//檔案類實體引數
            //圖片檔
            IFormFileCollection files = Request.Form.Files;


            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "無使用權限";
            }
            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            int ResultCode = _accountService.AddProduct(dto, files);

            return "[{\"st\": " + ResultCode + "}]";
        }

        //更新商品
        [HttpPut("UpdateProduct")]
        public string UpdateProduct()
        {
            //其他文字資訊
            //var dto = JsonConvert.DeserializeObject<ProductDto>(Request.Form["EditProductFrom"]);//檔案類實體引數
            OnlineShopBack.Domain.DTOs.Product.ProductDto dto = JsonSerializer.Deserialize<OnlineShopBack.Domain.DTOs.Product.ProductDto>(Request.Form["EditProductFrom"]);//檔案類實體引數
            //圖片檔
            IFormFileCollection files = Request.Form.Files;

            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "無使用權限";
            }



            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }
           

            int ResultCode = _accountService.UpdateProduct(dto, files);

            return "[{\"st\": " + ResultCode + "}]";
        }

        //刪除商品
        [HttpDelete("DelProduct")]
        public string DelProduct([FromQuery] int ProductId, string ProductNum, string ImgName)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "無使用權限";
            }


            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            int ResultCode = _accountService.DelProduct(ProductId, ProductNum, ImgName);

            return "[{\"st\": " + ResultCode + "}]";
        }

        /*---------------類別相關---------------*/

        //取得類別
        [HttpGet("GetCategory")]
        public string GetCategory()
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "未有使用權限";
            }


            DataTable dt = _accountService.GetCategory();

            ProductCategoryDto[] CategoryList = dt.Rows.Cast<DataRow>()
                 .Select(row => ProductCategoryDto.GetCategoryList(row))
                 .Where(Tuple => Tuple.Item1 == true)
                 .Select(Tuple => Tuple.Item2)
                 .ToArray();

            string Result = JsonSerializer.Serialize(CategoryList);//序列化回傳  回傳型態 string
            return Result;
        }

        //新增類別 
        [HttpPost("AddCategory")]
        public string AddCategory([FromBody] ProductCategoryDto value)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "無使用權限";
            }


            //資料驗證

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            int ResultCode = _accountService.AddCategory(value);

            
            return "[{\"st\": " + ResultCode + "}]";
        }

        //刪除類別
        [HttpDelete("DelCategory")]
        public string DelCategory([FromQuery] int Num, int SubNum)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "無使用權限";
            }


            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            int ResultCode = _accountService.DelCategory(Num, SubNum);

            return "[{\"st\": " + ResultCode + "}]";
        }

        //更新類別
        [HttpPut("UpdateCategory")]
        public string UpdateCategory([FromQuery] int Num, int SubNum, [FromBody] ProductCategoryDto value)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }
            else if (RolesValidate())
            {
                return "無使用權限";
            }

            

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            int ResultCode = _accountService.UpdateCategory(Num, SubNum, value);

            return "[{\"st\": " + ResultCode + "}]";

        }

        //---------------------------------------------------------

        //登入檢查
        private bool loginValidate()
        {
            if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Account")) ||                        //判斷Session[Account]是否為空
                SessionDB.sessionDB[HttpContext.Session.GetString("Account")].SId != HttpContext.Session.Id ||//判斷DB SessionId與瀏覽器 SessionId是否一樣
                SessionDB.sessionDB[HttpContext.Session.GetString("Account")].ValidTime < DateTime.Now)       //判斷是否過期
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        //權限檢查
        private bool RolesValidate()
        {

            if (HttpContext.Session.GetString("Roles").Contains("canUseMember"))
            {
                return false;
            }
            else
            {
                return true;
            }

        }

    }
}
