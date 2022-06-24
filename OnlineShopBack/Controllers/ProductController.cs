#region 功能與歷史修改描述
/*
    描述:商品系統相關
    日期:2022-05-09
*/
#endregion

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Services;
using OnlineShopBack.Domain.Tool;
using System;
using System.Data;
using System.IO;
using System.Text.Json;
using static OnlineShopBack.Enum.ProductEnum;
using OnlineShopBack.Domain.Repository;

namespace OnlineShopBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        //SQL連線字串  SQLConnectionString
        private string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");

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
            //DataTable轉Json;
            var result = MyTool.DataTableJson(dt);

            return result;
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

 

        //類別相關-----------------------------------------

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

            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds = new DataSet();
            try
            {
                // 資料庫連線&SQL指令
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);
                cmd.CommandText = @" EXEC pro_onlineShopBack_getProductCategory ";

                //開啟連線
                cmd.Connection.Open();
                da.SelectCommand = cmd;
                da.Fill(dt);
            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                //關閉連線
                if (cmd != null)
                {
                    cmd.Parameters.Clear();
                    cmd.Connection.Close();
                }
            }
            //DataTable轉Json;
            var result = MyTool.DataTableJson(dt);

            return result;
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

            string ErrorStr = "";//記錄錯誤訊息

            int[] CategoryArr = { 10, 20, 30 };//10=3C ,20=電腦周邊 ,30=軟體
            //主類別
            if (CategoryArr.IndexOf(value.CategoryNum) < 0)
            {
                ErrorStr += "[主類別]不存在請重新輸入\n";
            }

            //子類別
            if (value.SubCategoryNum > 999 || value.SubCategoryNum < 0)
            {
                ErrorStr += "[子類別編號應介於0～999之間]\n";
            }
            //子類別名稱
            if (string.IsNullOrEmpty(value.SubCategoryName))
            {
                ErrorStr += "[名稱不可為空]\n";
            }
            else
            {
                if (!MyTool.IsCNAndENAndNumber(value.SubCategoryName))
                {
                    ErrorStr += "[名稱應為中文,英文及數字]\n";
                }
                if (value.SubCategoryName.Length > 20 || value.SubCategoryName.Length < 0)
                {
                    ErrorStr += "[名稱應介於0～20個字之間]\n";
                }
            }

            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(ErrorStr))
            {
                return ErrorStr;
            }

            SqlCommand cmd = null;
            CategoryReturnCode result = CategoryReturnCode.Default;
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);

                cmd.CommandText = @"EXEC pro_onlineShopBack_addProductCategory @categoryNum, @subCategoryNum, @subCategoryName";

                cmd.Parameters.AddWithValue("@categoryNum", value.CategoryNum);
                cmd.Parameters.AddWithValue("@subCategoryNum", value.SubCategoryNum);
                cmd.Parameters.AddWithValue("@subCategoryName", value.SubCategoryName);
                //開啟連線
                cmd.Connection.Open();
                string SQLReturnCode = cmd.ExecuteScalar().ToString();//執行Transact-SQL


                if (!CategoryReturnCode.TryParse(SQLReturnCode, out result))
                {
                    result = CategoryReturnCode.Fail;
                }
            }
            catch (Exception e)
            {
                //TODO 要有log
                return e.Message;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Parameters.Clear();
                    cmd.Connection.Close();
                }
            }
            return "[{\"st\": " + (int)result + "}]";
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

            string ErrorStr = "";//記錄錯誤訊息

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            //主類別編號
            int[] CategoryArr = { 10, 20, 30 };//10=3C ,20=電腦周邊 ,30=軟體
            if (CategoryArr.IndexOf(Num) < 0)
            {
                ErrorStr += "[主類別]不存在請重新輸入\n";
            }

            //子類別編號 
            if (SubNum > 999 || SubNum < 0)
            {
                ErrorStr += "[子類別編號 應介於0～999之間]\n";
            }
            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(ErrorStr))
            {
                return ErrorStr;
            }

            SqlCommand cmd = null;
            CategoryReturnCode result = CategoryReturnCode.Default;
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);
                cmd.CommandText = @"EXEC pro_onlineShopBack_delCategory @categoryNum, @subCategoryNum";

                cmd.Parameters.AddWithValue("@categoryNum", Num);
                cmd.Parameters.AddWithValue("@subCategoryNum", SubNum);
                //開啟連線
                cmd.Connection.Open();
                string SQLReturnCode = cmd.ExecuteScalar().ToString();//執行Transact-SQL

                if (!CategoryReturnCode.TryParse(SQLReturnCode, out result))
                {
                    result = CategoryReturnCode.Fail;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Parameters.Clear();
                    cmd.Connection.Close();
                }
            }
            return "[{\"st\": " + (int)result + "}]";
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

            string ErrorStr = "";//記錄錯誤訊息

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            //主類別編號
            int[] CategoryArr = { 10, 20, 30 };//10=3C ,20=電腦周邊 ,30=軟體
            if (CategoryArr.IndexOf(Num) < 0)
            {
                ErrorStr += "[主類別]不存在請重新輸入\n";
            }

            //子類別編號 
            if (SubNum > 999 || SubNum < 0)
            {
                ErrorStr += "[子類別編號應介於0～999個之間]\n";
            }

            //權限名稱
            if (string.IsNullOrEmpty(value.SubCategoryName))
            {
                ErrorStr += "[子類別名稱不可為空]\n";
            }
            else
            {
                if (!MyTool.IsCNAndENAndNumber(value.SubCategoryName))
                {
                    ErrorStr += "[子類別名稱應為中文,英文及數字]\n";
                }
                if (value.SubCategoryName.Length > 20 || value.SubCategoryName.Length < 0)
                {
                    ErrorStr += "[子類別名稱應介於0～20個字之間]\n";
                }
            }

            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(ErrorStr))
            {
                return ErrorStr;
            }

            SqlCommand cmd = null;
            CategoryReturnCode result = CategoryReturnCode.Default;
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);

                cmd.CommandText = @"EXEC pro_onlineShopBack_putCategory @num, @subNum, @subCategoryName";

                cmd.Parameters.AddWithValue("@num", Num);
                cmd.Parameters.AddWithValue("@subNum", SubNum);
                cmd.Parameters.AddWithValue("@subCategoryName", value.SubCategoryName);

                //開啟連線
                cmd.Connection.Open();
                string SQLReturnCode = cmd.ExecuteScalar().ToString();//執行Transact-SQL                

                if (!CategoryReturnCode.TryParse(SQLReturnCode, out result))
                {
                    result = CategoryReturnCode.Fail;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Parameters.Clear();
                    cmd.Connection.Close();
                }
            }

            return "[{\"st\": " + (int)result + "}]";

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
