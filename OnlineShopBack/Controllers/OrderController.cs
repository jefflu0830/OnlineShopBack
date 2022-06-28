#region 功能與歷史修改描述
/*
    描述:訂單系統相關
    日期:2022-05-05
*/
#endregion

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OnlineShopBack.Services;
using OnlineShopBack.Domain.Tool;
using System;
using System.Data;
using System.IO;
using static OnlineShopBack.Enum.OrderEnum;
using OnlineShopBack.Domain.DTOs.Order;
using System.Text.Json;
using System.Linq;
using OnlineShopBack.Domain.Repository;

namespace OnlineShopBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderService = null;
        public OrderController(IOrderRepository OrderService)
        {
            _orderService = OrderService;
        }

        /*-----------------訂單相關-----------------*/

        //取得訂單
        [HttpGet("GetOrder")]
        public string GetOrder()
        {

            DataSet st = _orderService.GetOrder();

            OrderDto[] OrderList = st.Tables[0].Rows.Cast<DataRow>()
                 .Select(row => OrderDto.GetOrderList(row))
                 .Where(accTuple => accTuple.Item1 == true)
                 .Select(accTuple => accTuple.Item2)
                 .ToArray();

            TransportDto[] TransportList = st.Tables[1].Rows.Cast<DataRow>()
                  .Select(row => TransportDto.GetTransportList(row))
                  .Where(accTuple => accTuple.Item1 == true)
                  .Select(accTuple => accTuple.Item2)
                  .ToArray();

            TransportDto[] TransportStatusList = st.Tables[2].Rows.Cast<DataRow>()
                   .Select(row => TransportDto.GetTransportStatusList(row))
                   .Where(accTuple => accTuple.Item1 == true)
                   .Select(accTuple => accTuple.Item2)
                   .ToArray();

            string OrderTable = "\"OrderTable\":" + JsonSerializer.Serialize(OrderList);
            string TransportTable = "\"TransportTable\":" + JsonSerializer.Serialize(TransportList);
            string TransportStatusTable = "\"TransportStatusTable\":" + JsonSerializer.Serialize(TransportStatusList);

            return "{" + OrderTable + "," + TransportTable + "," + TransportStatusTable + "}";

        }

        //更新訂單配送方式&狀態
        [HttpPut("UpdateOrder")]
        public string UpdateOrder([FromQuery] string OrderNum, int TransportNum, int TransportStatusNum)
        {


            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            int ResultCode = _orderService.UpdateOrder(OrderNum, TransportNum, TransportStatusNum);

            return "[{\"st\": " + ResultCode + "}]";

        }

        //訂單退貨
        [HttpPut("OrderReturn")]
        public string OrderReturn([FromQuery] string OrderNum)
        {

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            int ResultCode = _orderService.OrderReturn(OrderNum);

            return "[{\"st\": " + ResultCode + "}]";

        }

        //取消訂單
        [HttpPut("OrderCancel")]
        public string OrderCancel([FromQuery] string OrderNum)
        {

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            int ResultCode = _orderService.OrderCancel(OrderNum);

            return "[{\"st\": " + ResultCode + "}]";

        }

        /*配送方式相關-----------------*/

        //取得配送方式
        [HttpGet("GetTransport")]
        public string GetTransport()
        {
            DataTable dt = _orderService.GetTransport();

            TransportDto[] TransportList = dt.Rows.Cast<DataRow>()
                 .Select(row => TransportDto.GetTransportList(row))
                 .Where(accTuple => accTuple.Item1 == true)
                 .Select(accTuple => accTuple.Item2)
                 .ToArray();

            string TransportTable = "";//"\"OrderTable\":" + JsonSerializer.Serialize(TransportList);
            //DataTable轉Json;
            //var TransportTable = "\"TransportTable\":" + MyTool.DataTableJson(dt);

            return "{" + TransportTable + "}";
        }

        //新增配送方式
        [HttpPost("AddTransport")]
        public string AddTransport([FromBody] TransportDto value)
        {

            //資料驗證
            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            string ErrorStr = "";//記錄錯誤訊息

            //編號 
            if (value.Transport > 255 || value.Transport < 0)
            {
                ErrorStr += "[代號應介於0～255個之間]\n";
            }

            //名稱
            if (string.IsNullOrEmpty(value.TransportName))
            {
                ErrorStr += "[名稱不可為空]\n";
            }
            else
            {
                if (!MyTool.IsCNAndENAndNumber(value.TransportName))
                {
                    ErrorStr += "[名稱應為中文,英文及數字]\n";
                }
                if (value.TransportName.Length > 20 || value.TransportName.Length < 1)
                {
                    ErrorStr += "[名稱應介於1～20個字之間]\n";
                }
            }

            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(ErrorStr))
            {
                return ErrorStr;
            }

            SqlCommand cmd = null;
            OrderReturnCode result = OrderReturnCode.Default;
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection("");

                cmd.CommandText = @"EXEC pro_onlineShopBack_addTransport @transport, @transportName ";

                cmd.Parameters.AddWithValue("@transport", value.Transport);
                cmd.Parameters.AddWithValue("@transportName", value.TransportName);

                //開啟連線
                cmd.Connection.Open();
                string SQLReturnCode = cmd.ExecuteScalar().ToString();//執行Transact-SQL


                if (!OrderReturnCode.TryParse(SQLReturnCode, out result))
                {
                    result = OrderReturnCode.Fail;
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

            //編號 
            if (TransportNum > 255 || TransportNum < 0)
            {
                ErrorStr += "[代號應介於0～255個之間]\n";
            }

            //名稱
            if (string.IsNullOrEmpty(TransportName))
            {
                ErrorStr += "[名稱不可為空]\n";
            }
            else
            {
                if (!MyTool.IsCNAndENAndNumber(TransportName))
                {
                    ErrorStr += "[名稱應為中文,英文及數字]\n";
                }
                if (TransportName.Length > 20 || TransportName.Length < 0)
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
            OrderReturnCode result = OrderReturnCode.Default;
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection("");

                cmd.CommandText = @"EXEC pro_onlineShopBack_putTransport @TransportNum, @TransportName ";

                cmd.Parameters.AddWithValue("@transportNum", TransportNum);
                cmd.Parameters.AddWithValue("@transportName", TransportName);

                //開啟連線
                cmd.Connection.Open();
                string SQLReturnCode = cmd.ExecuteScalar().ToString();//執行Transact-SQL                

                if (!OrderReturnCode.TryParse(SQLReturnCode, out result))
                {
                    result = OrderReturnCode.Fail;
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

            //編號 
            if (TransportNum > 255 || TransportNum < 0)
            {
                ErrorStr += "[代號應介於0～255個之間]\n";
            }

            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(ErrorStr))
            {
                return ErrorStr;
            }

            SqlCommand cmd = null;
            OrderReturnCode result = OrderReturnCode.Default;
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection("");
                cmd.CommandText = @"EXEC pro_onlineShopBack_delTransport @transportNum ";

                cmd.Parameters.AddWithValue("@transportNum", TransportNum);
                //開啟連線
                cmd.Connection.Open();
                string SQLReturnCode = cmd.ExecuteScalar().ToString();//執行Transact-SQL

                if (!OrderReturnCode.TryParse(SQLReturnCode, out result))
                {
                    result = OrderReturnCode.Fail;
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

        /*配送狀態相關-----------------*/


        //取得配送狀態
        [HttpGet("GetTransportStatus")]
        public string GetTransportStatus()
        {

            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                // 資料庫連線&SQL指令
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection("");
                cmd.CommandText = @" SELECT f_transport, f_transportName FROM t_transport  
                                     SELECT f_transport, f_transportStatus,f_transportStatusName FROM t_transportStatus ";

                //開啟連線
                cmd.Connection.Open();
                da.SelectCommand = cmd;
                da.Fill(ds);
            }
            catch (Exception e)
            {
                MyTool.WriteErroLog(e.Message);
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
            var TransportTable = "\"TransportTable\":" + MyTool.DataTableJson(ds.Tables[0]);
            var TransportStatusTable = "\"TransportStatusTable\":" + MyTool.DataTableJson(ds.Tables[1]);

            return "{" + TransportTable + "," + TransportStatusTable + "}";
        }

        //新增配送狀態
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

            //編號 
            if (value.Transport > 255 || value.Transport < 0 ||
                value.TransportStatus > 255 || value.TransportStatus < 0)
            {
                ErrorStr += "[代號應介於0～255個之間]\n";
            }

            //名稱
            if (string.IsNullOrEmpty(value.TransportStatusName))
            {
                ErrorStr += "[名稱不可為空]\n";
            }
            else
            {
                if (!MyTool.IsCNAndENAndNumber(value.TransportStatusName))
                {
                    ErrorStr += "[名稱應為中文,英文及數字]\n";
                }
                if (value.TransportStatusName.Length > 20 || value.TransportStatusName.Length < 1)
                {
                    ErrorStr += "[名稱應介於1～20個字之間]\n";
                }
            }


            //錯誤訊息有值 return錯誤值
            if (!string.IsNullOrEmpty(ErrorStr))
            {
                return ErrorStr;
            }

            SqlCommand cmd = null;
            OrderReturnCode result = OrderReturnCode.Default;
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection("");

                cmd.CommandText = @"EXEC pro_onlineShopBack_addTransportStatus @transport, @transportStatus, @transportName ";

                cmd.Parameters.AddWithValue("@transport", value.Transport);
                cmd.Parameters.AddWithValue("@transportStatus", value.TransportStatus);
                cmd.Parameters.AddWithValue("@transportName", value.TransportStatusName);

                //開啟連線
                cmd.Connection.Open();
                string SQLReturnCode = cmd.ExecuteScalar().ToString();//執行Transact-SQL


                if (!OrderReturnCode.TryParse(SQLReturnCode, out result))
                {
                    result = OrderReturnCode.Fail;
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
        [HttpPut("UpdateTransportStatus")]
        public string UpdateTransportStatus([FromQuery] int TransportNum, int TransportStatusNum, string TransportStatusName)
        {
            string ErrorStr = "";//記錄錯誤訊息

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            //編號 
            if (TransportNum > 255 || TransportNum < 0 ||
                TransportStatusNum > 255 || TransportStatusNum < 0)
            {
                ErrorStr += "[代號應介於0～255個之間]\n";
            }

            //名稱
            if (string.IsNullOrEmpty(TransportStatusName))
            {
                ErrorStr += "[名稱不可為空]\n";
            }
            else
            {
                if (!MyTool.IsCNAndENAndNumber(TransportStatusName))
                {
                    ErrorStr += "[名稱應為中文,英文及數字]\n";
                }
                if (TransportStatusName.Length > 20 || TransportStatusName.Length < 0)
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
            OrderReturnCode result = OrderReturnCode.Default;
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection("");

                cmd.CommandText = @"EXEC pro_onlineShopBack_putTransportStatus @TransportNum, @transportStatusNum, @transportStatusName ";

                cmd.Parameters.AddWithValue("@transportNum", TransportNum);
                cmd.Parameters.AddWithValue("@transportStatusNum", TransportStatusNum);
                cmd.Parameters.AddWithValue("@transportStatusName", TransportStatusName);

                //開啟連線
                cmd.Connection.Open();
                string SQLReturnCode = cmd.ExecuteScalar().ToString();//執行Transact-SQL                

                if (!OrderReturnCode.TryParse(SQLReturnCode, out result))
                {
                    result = OrderReturnCode.Fail;
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

        //刪除配送狀態
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
            OrderReturnCode result = OrderReturnCode.Default;
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection("");
                cmd.CommandText = @"EXEC pro_onlineShopBack_delTransportStatus @transportNum, @transportStatusNum ";

                cmd.Parameters.AddWithValue("@transportNum", TransportNum);
                cmd.Parameters.AddWithValue("@transportStatusNum", TransportStatusNum);
                //開啟連線
                cmd.Connection.Open();
                string SQLReturnCode = cmd.ExecuteScalar().ToString();//執行Transact-SQL

                if (!OrderReturnCode.TryParse(SQLReturnCode, out result))
                {
                    result = OrderReturnCode.Fail;
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
