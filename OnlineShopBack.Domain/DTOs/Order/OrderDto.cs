using System;
using System.Data;

namespace OnlineShopBack.Domain.DTOs.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string OrderNum { get; set; }
        public int MemberId { get; set; }
        public string MemberAcc { get; set; }
        public DateTime OrderDate { get; set; }
        public int Transport { get; set; }
        public int TransportStatus { get; set; }
        public int OrderStatus { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public static (bool, OrderDto) GetOrderList(DataRow row)
        {
            OrderDto temp = new OrderDto();

            temp.Id = (int)row["f_id"];
            temp.OrderNum = row["f_orderNum"].ToString();
            temp.MemberAcc = row["f_acc"].ToString();
            temp.OrderDate = Convert.ToDateTime(row["f_orderDate"].ToString());
            temp.Transport = (int)row["f_transport"];
            temp.TransportStatus = (int)row["f_transportStatus"];
            temp.OrderStatus = (int)row["f_orderStatus"];

            return (true, temp);
        }

    }
}
