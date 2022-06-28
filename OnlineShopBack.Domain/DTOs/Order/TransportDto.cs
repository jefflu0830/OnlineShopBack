using System;
using System.Data;

namespace OnlineShopBack.Domain.DTOs.Order
{
    public class TransportDto
    {

        public int Transport { get; set; }
        public string TransportName { get; set; }
        public int TransportStatus { get; set; }
        public string TransportStatusName { get; set; }

        public static (bool, TransportDto) GetTransportList(DataRow row)
        {
            TransportDto temp = new TransportDto();

            temp.Transport = (int)row["f_transport"];
            temp.TransportName = row["f_transportName"].ToString();

            return (true, temp);
        }
        public static (bool, TransportDto) GetTransportStatusList(DataRow row)
        {
            TransportDto temp = new TransportDto();

            temp.Transport = (int)row["f_transport"];
            temp.TransportStatus = (int)row["f_transportStatus"];
            temp.TransportStatusName = row["f_transportStatusName"].ToString();

            return (true, temp);
        }
    }
}
