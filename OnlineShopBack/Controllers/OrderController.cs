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

            int ResultCode = _orderService.AddTransport(value);

            return "[{\"st\": " + ResultCode + "}]";
        }

        //更新配送方式名稱
        [HttpPut("UpdateTransport")]
        public string UpdateTransport([FromQuery] int TransportNum, string TransportName)
        {
            

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            int ResultCode = _orderService.UpdateTransport(TransportNum, TransportName);

            return "[{\"st\": " + ResultCode + "}]";

        }

        //刪除配送方式
        [HttpDelete("DelTransport")]
        public string DelTransport([FromQuery] int TransportNum)
        {           

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            int ResultCode = _orderService.DelTransport(TransportNum);
            return "[{\"st\": " + ResultCode + "}]";
        }

        /*配送狀態相關-----------------*/


        //取得配送狀態
        [HttpGet("GetTransportStatus")]
        public string GetTransportStatus()
        {
            DataSet ds = _orderService.GetTransportStatus();

            TransportDto[] TransportList = ds.Tables[0].Rows.Cast<DataRow>()
                .Select(row => TransportDto.GetTransportList(row))
                .Where(accTuple => accTuple.Item1 == true)
                .Select(accTuple => accTuple.Item2)
                .ToArray();

            TransportDto[] TransportStatusList = ds.Tables[1].Rows.Cast<DataRow>()
                .Select(row => TransportDto.GetTransportStatusList(row))
                .Where(accTuple => accTuple.Item1 == true)
                .Select(accTuple => accTuple.Item2)
                .ToArray();


            //DataTable轉Json;
            string TransportTable = "\"TransportTable\":" + JsonSerializer.Serialize(TransportList);
            string TransportStatusTable = "\"TransportStatusTable\":" + JsonSerializer.Serialize(TransportStatusList);

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

            int ResultCode = _orderService.AddTransportStatus(value);

           
            return "[{\"st\": " + ResultCode + "}]";
        }

        //更新配送方式名稱
        [HttpPut("UpdateTransportStatus")]
        public string UpdateTransportStatus([FromQuery] int TransportNum, int TransportStatusNum, string TransportStatusName)
        {
            

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            int ResultCode = _orderService.UpdateTransportStatus(TransportNum, TransportStatusNum, TransportStatusName);

            return "[{\"st\": " + ResultCode + "}]";

        }

        //刪除配送狀態
        [HttpDelete("DelTransportStatus")]
        public string DelTransportStatus([FromQuery] int TransportNum, int TransportStatusNum)
        {           

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            int ResultCode = _orderService.DelTransportStatus(TransportNum, TransportStatusNum);
            
            return "[{\"st\": " + ResultCode + "}]";
        }

    }
}
