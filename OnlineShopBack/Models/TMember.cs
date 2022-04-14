using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace OnlineShopBack.Models
{
    public partial class TMember
    {
        public int FId { get; set; }
        public string FAcc { get; set; }
        public string FPwd { get; set; }
        public string FPhone { get; set; }
        public string FMail { get; set; }
        public string FName { get; set; }
        public string FAddress { get; set; }
        public int FShopGold { get; set; }
        public byte FLevel { get; set; }
        public byte FSuspension { get; set; }
        public DateTime FCreateDate { get; set; }
        public DateTime FUpdateDate { get; set; }
    }
}
