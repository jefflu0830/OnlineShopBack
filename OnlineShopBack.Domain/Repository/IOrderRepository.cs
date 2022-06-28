using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace OnlineShopBack.Domain.Repository
{
    public interface IOrderRepository
    {
        /*-----------------訂單相關-----------------*/

        public DataSet GetOrder();//取得訂單

        public int UpdateOrder(string OrderNum, int TransportNum, int TransportStatusNum);//更新訂單配送方式&狀態

        public int OrderReturn(string OrderNum);//訂單退貨

        public int OrderCancel(string OrderNum);//取消訂單

        /*配送方式相關-----------------*/

        public DataTable GetTransport();//取得配送方式
    }
}
