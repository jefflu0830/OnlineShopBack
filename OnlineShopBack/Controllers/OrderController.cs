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

namespace OnlineShopBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        //SQL連線字串  SQLConnectionString
        private string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");

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
            var OrderTable = "\"OrderTable\":"+ MyTool.DataTableJson(st.Tables[0]);
            var TransportTable = "\"TransportTable\":" + MyTool.DataTableJson(st.Tables[1]);
            var TransportStatusTable = "\"TransportStatusTable\":" + MyTool.DataTableJson(st.Tables[2]);

            return "{"+ OrderTable +","+ TransportTable + ","+ TransportStatusTable + "}";
        }

    }
}
