#region 功能與歷史修改描述
/*
    描述:後台訂單系統相關
    日期:2022-05-05
*/
#endregion

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Services;
using OnlineShopBack.Tool;
using System;
using System.Data;
using static OnlineShopBack.Enum.OrderEnum;

namespace OnlineShopBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        //SQL連線字串  SQLConnectionString
        private string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");

        //訂單相關-----------------

        //取得訂單
        [HttpGet("GetProduct")]
        public string GetProduct()
        {

            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            DataSet st = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                // 資料庫連線&SQL指令
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);
                cmd.CommandText = @" EXEC pro_onlineShopBack_getOrder ";

                //開啟連線
                cmd.Connection.Open();
                da.SelectCommand = cmd;
                da.Fill(st);
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

            var OrderTable = "\"OrderTable\":" + MyTool.DataTableJson(st.Tables[0]);
            var TransportTable = "\"TransportTable\":" + MyTool.DataTableJson(st.Tables[1]);
            var TransportStatusTable = "\"TransportStatusTable\":" + MyTool.DataTableJson(st.Tables[2]);

            return "{" + OrderTable + "," + TransportTable + "," + TransportStatusTable + "}";
        }

        //配送方式相關-----------------

        //新增配送方式
        [HttpPost("AddTransport")]
        public string AddTransport([FromBody] TransportDto value )
        {

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
            TransportReturnCode result = TransportReturnCode.Default;
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);

                cmd.CommandText = @"EXEC pro_onlineShopBack_addTransport @transport, @transportName ";

                cmd.Parameters.AddWithValue("@transport", value.Transport);
                cmd.Parameters.AddWithValue("@transportName", value.TransportName);

                //開啟連線
                cmd.Connection.Open();
                string SQLReturnCode = cmd.ExecuteScalar().ToString();//執行Transact-SQL


                if (!TransportReturnCode.TryParse(SQLReturnCode, out result))
                {
                    result = TransportReturnCode.Fail;
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

        //更新配送方式名稱
        [HttpPut("UpdateTransport")]
        public string UpdateTransport([FromQuery] int TransportNum, string TransportName)
        {
            string ErrorStr = "";//記錄錯誤訊息

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            //子類別編號 
            if (TransportNum > 256 || TransportNum < 0)
            {
                ErrorStr += "[代號應介於0～256個之間]\n";
            }

            //權限名稱
            if (string.IsNullOrEmpty(TransportName))
            {
                ErrorStr += "[子類別名稱不可為空]\n";
            }
            else
            {
                if (!MyTool.IsCNAndENAndNumber(TransportName))
                {
                    ErrorStr += "[子類別名稱應為中文,英文及數字]\n";
                }
                if (TransportName.Length > 20 || TransportName.Length < 0)
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
            TransportReturnCode result = TransportReturnCode.Default;
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);

                cmd.CommandText = @"EXEC pro_onlineShopBack_putTransport @TransportNum, @TransportName ";

                cmd.Parameters.AddWithValue("@transportNum", TransportNum);
                cmd.Parameters.AddWithValue("@transportName", TransportName);

                //開啟連線
                cmd.Connection.Open();
                string SQLReturnCode = cmd.ExecuteScalar().ToString();//執行Transact-SQL                

                if (!TransportReturnCode.TryParse(SQLReturnCode, out result))
                {
                    result = TransportReturnCode.Fail;
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

        //刪除配送方式
        [HttpDelete("DelTransport")]
        public string DelTransport([FromQuery] int TransportNum)
        {

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
            TransportReturnCode result = TransportReturnCode.Default;
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);
                cmd.CommandText = @"EXEC pro_onlineShopBack_delTransport @transportNum ";

                cmd.Parameters.AddWithValue("@transportNum", TransportNum);
                //開啟連線
                cmd.Connection.Open();
                string SQLReturnCode = cmd.ExecuteScalar().ToString();//執行Transact-SQL

                if (!TransportReturnCode.TryParse(SQLReturnCode, out result))
                {
                    result = TransportReturnCode.Fail;
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

        //配送狀態相關-----------------

        //新增配送方式
        [HttpPost("AddTransportStatus")]
        public string AddTransportStatus([FromBody] TransportDto value)
        {

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
            TransportReturnCode result = TransportReturnCode.Default;
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);

                cmd.CommandText = @"EXEC pro_onlineShopBack_addTransportStatus @transport, @transportStatus, @transportName ";

                cmd.Parameters.AddWithValue("@transport", value.Transport);
                cmd.Parameters.AddWithValue("@transportStatus", value.TransportStatus);
                cmd.Parameters.AddWithValue("@transportName", value.TransportStatusName);

                //開啟連線
                cmd.Connection.Open();
                string SQLReturnCode = cmd.ExecuteScalar().ToString();//執行Transact-SQL


                if (!TransportReturnCode.TryParse(SQLReturnCode, out result))
                {
                    result = TransportReturnCode.Fail;
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

        //刪除配送方式
        [HttpDelete("DelTransportStatus")]
        public string DelTransportStatus([FromQuery] int TransportNum, int TransportStatusNum)
        {

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
            TransportReturnCode result = TransportReturnCode.Default;
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);
                cmd.CommandText = @"EXEC pro_onlineShopBack_delTransportStatus @transportNum, @transportStatusNum ";

                cmd.Parameters.AddWithValue("@transportNum", TransportNum);
                cmd.Parameters.AddWithValue("@transportStatusNum", TransportStatusNum);
                //開啟連線
                cmd.Connection.Open();
                string SQLReturnCode = cmd.ExecuteScalar().ToString();//執行Transact-SQL

                if (!TransportReturnCode.TryParse(SQLReturnCode, out result))
                {
                    result = TransportReturnCode.Fail;
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

    }
}
