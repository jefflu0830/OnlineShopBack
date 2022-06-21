using System;

namespace OnlineShopBack.Domain.DTOs.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string OrderNum { get; set; }
        public int MemberId { get; set; }
        public DateTime OrderDate { get; set; }
        public int Transport { get; set; }
        public int TransportStatus { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}
