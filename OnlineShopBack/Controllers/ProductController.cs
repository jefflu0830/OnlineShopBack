#region 功能與歷史修改描述
/*
    描述:商品系統相關
    日期:2022-05-09
*/
#endregion

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Services;
using OnlineShopBack.Tool;
using System;
using Microsoft.EntityFrameworkCore.Internal;
using System.Data;

namespace OnlineShopBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        //SQL連線字串  SQLConnectionString
        private string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");


        //商品相關---------------------------------------------

        #region 商品相關列舉(Enum)
        private enum ProductReturnCode //新增商品
        {
            //<summary >
            //新增成功
            //</summary >
            AddOK = 0,
            //<summary >
            //商品重複新增
            //</summary >
            DuplicateProduct = 100,
            //<summary >
            //商品類型不存在
            //</summary >
            CategoryIsNull = 101
        }
        #endregion

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

            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds = new DataSet();
            try
            {
                // 資料庫連線&SQL指令
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);
                cmd.CommandText = @" EXEC pro_onlineShopBack_getProductAndCategory ";

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

        //新增商品 
        [HttpPost("AddProduct")]
        public string AddProduct([FromBody] ProductDto value)
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

            //資料驗證-----------------------------------------------
            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            string ErrorStr = "";//記錄錯誤訊息

            //商品代號
            if (string.IsNullOrWhiteSpace(value.Num))
            {
                ErrorStr += "[商品代號不可為空]\n";
            }
            else
            {
                if (!MyTool.IsENAndNumber(value.Num))
                {
                    ErrorStr += "[商品代號只能為英數]\n";
                }
                if (value.Num.Length > 20 || value.Num.Length < 3)
                {
                    ErrorStr += "[商品代號長度應介於3～20個數字之間]\n";
                }
            }
            //商品主類別編號
            int[] CategoryArr = { 10, 20, 30 };//10=3C ,20=電腦周邊 ,30=軟體
            if (CategoryArr.IndexOf(value.Category) < 0)
            {
                ErrorStr += "[主類別]不存在請重新輸入\n";
            }
            //商品子類型
            if (value.SubCategory > 999 || value.SubCategory < 0)
            {
                ErrorStr += "[商品類型&商品子類型範圍應在0-999之間]\n";
            }
            //商品名稱
            if (string.IsNullOrWhiteSpace(value.Name))
            {
                ErrorStr += "[商品名稱不可為空]\n";
            }
            else
            {
                if (!MyTool.IsCNAndENAndNumber(value.Name))
                {
                    ErrorStr += "[商品名稱應為中文,英文及數字]\n";
                }
                if (value.Name.Length > 20 || value.Name.Length < 1)
                {
                    ErrorStr += "[商品名稱應介於1～20個字之間]\n";
                }
            }
            //圖片
            if (value.ImgPath.Length > 100 || value.ImgPath.Length < 0)
            {
                ErrorStr += "[圖片地址長度應介於1～100個字之間]\n";
            }
            //價格
            if (value.Price < 0 || value.Price > 999999999)
            {
                ErrorStr += "[價格不得為負或大於999999999]\n";
            }
            //是否開放商品
            if ((value.Status != 0 && value.Status != 100))//|| (value.Status != 0))
            {
                ErrorStr += "[狀態碼應為0(開放)或100(不開放)]\n";
            }
            //商品描述
            if (!string.IsNullOrWhiteSpace(value.Name))
            {//不為空才要做字數判斷
                if (value.Content.Length > 500 || value.Content.Length < 0)
                {
                    ErrorStr += "[商品描述應在500字內]\n";
                }
            }
            //庫存量
            if (value.Stock < 1 || value.Stock > 99999)
            {
                ErrorStr += "[庫存量應介於1-99999]\n";
            }


            //ErrorStr不為空 則回傳錯誤訊息
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

                cmd.CommandText = @"EXEC pro_onlineShopBack_addProduct @num, @category, @subCategory, @name, @ImgPath, @price, @status, @contnet, @stock";

                cmd.Parameters.AddWithValue("@num", value.Num);                //商品代號
                cmd.Parameters.AddWithValue("@category", value.Category);      //商品類型
                cmd.Parameters.AddWithValue("@subCategory", value.SubCategory);//商品子類型
                cmd.Parameters.AddWithValue("@name", value.Name);              //商品名稱
                cmd.Parameters.AddWithValue("@ImgPath", value.ImgPath);        //圖片
                cmd.Parameters.AddWithValue("@price", value.Price);            //價格
                cmd.Parameters.AddWithValue("@status", value.Status);          //開放狀態
                cmd.Parameters.AddWithValue("@contnet", value.Content);        //商品描述     
                cmd.Parameters.AddWithValue("@stock", value.Stock);            //庫存量

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

        //刪除商品
        [HttpDelete("DelProduct")]
        public string DelCategory([FromQuery] int ProductId, string ProductNum)
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
                cmd.CommandText = @"EXEC pro_onlineShopBack_delProduct @productId, @productNum";

                cmd.Parameters.AddWithValue("@productId", ProductId);
                cmd.Parameters.AddWithValue("@productNum", ProductNum);
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
        [HttpPut("UpdateProduct")]
        public string UpdateProduct([FromBody] ProductDto value)
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

                cmd.CommandText = @"EXEC pro_onlineShopBack_putProduct @productNum, @category, @subCategory, @name, @ImgPath, @price, @status, @contnet, @stock";


                cmd.Parameters.AddWithValue("@productNum", value.Num);          //id
                cmd.Parameters.AddWithValue("@category", value.Category);      //商品類型
                cmd.Parameters.AddWithValue("@subCategory", value.SubCategory);//商品子類型
                cmd.Parameters.AddWithValue("@name", value.Name);              //商品名稱
                cmd.Parameters.AddWithValue("@ImgPath", value.ImgPath);        //圖片
                cmd.Parameters.AddWithValue("@price", value.Price);            //價格
                cmd.Parameters.AddWithValue("@status", value.Status);          //開放狀態
                cmd.Parameters.AddWithValue("@contnet", value.Content);        //商品描述     
                cmd.Parameters.AddWithValue("@stock", value.Stock);            //庫存量

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

        //類別相關-----------------------------------------

        #region 類別相關列舉(Enum)
        private enum CategoryReturnCode//[類別]相關代號
        {
            //<summary >
            //成功
            //</summary >
            Success = 0,
            //<summary >
            //失敗
            //</summary >
            Fail = 1,
            //<summary >
            //預設值
            //</summary >
            Default = 2
        }
        #endregion

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
                //cmd.CommandText = @"EXEC pro_onlineShopBack_getAccountAndAccountLevel";
                cmd.CommandText = @" EXEC pro_onlineShopBack_getProductCategory ";

                //開啟連線
                cmd.Connection.Open();
                da.SelectCommand = cmd;
                da.Fill(dt);

                da.Fill(ds);


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
