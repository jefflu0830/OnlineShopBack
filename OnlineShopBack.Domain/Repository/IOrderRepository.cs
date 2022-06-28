using OnlineShopBack.Domain.DTOs.Order;
using System.Data;

namespace OnlineShopBack.Domain.Repository
{
    public interface IOrderRepository
    {
        /*-----------------訂單相關-----------------*/

        public DataSet GetOrder();//取得訂單

        public int UpdateOrder(string OrderNum, int TransportNum, int TransportStatusNum);//更新訂單配送方式&狀態

        public int OrderReturn(string OrderNum);//訂單退貨

        public int OrderCancel(string OrderNum);//取消訂單

        /*-----------------配送方式相關-----------------*/

        public DataTable GetTransport();//取得配送方式

        public int AddTransport(TransportDto value);//新增配送方式

        public int DelTransport(int TransportNum); //刪除配送方式

        public int UpdateTransport(int TransportNum, string TransportName);//更新配送方式名稱

        /*-----------------配送狀態相關-----------------*/

        public DataSet GetTransportStatus();//取得配送狀態

        public int AddTransportStatus(TransportDto value);//新增配送狀態

        public int UpdateTransportStatus(int TransportNum, int TransportStatusNum, string TransportStatusName);//更新配送方式名稱

        public int DelTransportStatus(int TransportNum, int TransportStatusNum);//刪除配送狀態
    }
}
