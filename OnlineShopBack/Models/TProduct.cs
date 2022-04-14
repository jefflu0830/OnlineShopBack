using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace OnlineShopBack.Models
{
    public partial class TProduct
    {
        public int FId { get; set; }
        public string FNum { get; set; }
        public int FCategory { get; set; }
        public string FName { get; set; }
        public string FImg { get; set; }
        public int FPrice { get; set; }
        public byte FStatus { get; set; }
        public string FContent { get; set; }
        public int FStock { get; set; }
        public DateTime FCreateDate { get; set; }
        public DateTime FUpdateDate { get; set; }
    }
}
