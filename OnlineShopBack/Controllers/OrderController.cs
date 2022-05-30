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

        //訂單相關----------------------------------
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
                //return e.Message;
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


        //配送相關----------------------------------
        //新增配送
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


    }
}
