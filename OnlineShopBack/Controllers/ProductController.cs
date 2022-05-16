using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Services;
using OnlineShopBack.Tool;
using System;
using OnlineShopBack.DTOs;
using Microsoft.EntityFrameworkCore.Internal;

namespace OnlineShopBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        //SQL連線字串  SQLConnectionString
        private string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");

        #region 商品相關列舉(Enum)
        private enum AddProductReturnCode //新增商品
        {
            //<summary >
            //新增成功
            //</summary >
            AddOK = 0,
            //<summary >
            //商品重複新增
            //</summary >
            DuplicateProduct = 100

        }
        #endregion
        //商品相關----------------------------
        //新增商品 
        [HttpPost("AddProduct")]
        public string AddProduct([FromBody] MemLvDto value)
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

            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(ErrorStr))
            {
                return ErrorStr;
            }

            SqlCommand cmd = null;
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);

                cmd.CommandText = @"EXEC pro_onlineShopBack_addMemberLevel @memLv, @LvName";

                cmd.Parameters.AddWithValue("@memLv", value.memLv);
                cmd.Parameters.AddWithValue("@LvName", value.LvName);

                //開啟連線
                cmd.Connection.Open();
                string ReturnCode = cmd.ExecuteScalar().ToString();//執行Transact-SQL
                int SQLReturnCode = int.Parse(ReturnCode);

                switch (SQLReturnCode)
                {

                    case (int)AddProductReturnCode.AddOK:
                        return "商品新增成功";
                    case (int)AddProductReturnCode.DuplicateProduct:
                        return "已經有一樣的商品了";
                    default:
                        return "失敗";
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
        }

        //商品類別相關------------------------
        //新增商品類別 
        [HttpPost("AddCategory")]
        public string AddCategory([FromBody] ProductCategory  value)
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

            int[] CategoryArr = { 10, 20, 30 };

            if (CategoryArr.IndexOf(value.CategoryNum) < 0)
            {
                ErrorStr += "[主類別]不存在請重新輸入\n";
            }

            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(ErrorStr))
            {
                return ErrorStr;
            }

            SqlCommand cmd = null;
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);

                cmd.CommandText = @"EXEC pro_onlineShopBack_addProductCategory @CategoryNum, @SubCategoryNum, @SubCategoryName";

                cmd.Parameters.AddWithValue("@CategoryNum", value.CategoryNum);
                cmd.Parameters.AddWithValue("@SubCategoryNum", value.SubCategoryNum);
                cmd.Parameters.AddWithValue("@SubCategoryName", value.SubCategoryName);
                //開啟連線
                cmd.Connection.Open();
                string ReturnCode = cmd.ExecuteScalar().ToString();//執行Transact-SQL
                int SQLReturnCode = int.Parse(ReturnCode);

                switch (SQLReturnCode)
                {

                    case (int)AddProductReturnCode.AddOK:
                        return "子類別新增成功";
                    case (int)AddProductReturnCode.DuplicateProduct:
                        return "子類別代號重複";
                    default:
                        return "失敗";
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
        }

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
