using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopBack.Controllers
{
    public class PutShopGlodDto
    {
        public string MemAcc { get; set; }
        public int? NowAmount { get; set; }
        public int? AdjustAmount { get; set; }

    }
}