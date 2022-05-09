using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopBack.Controllers
{
    public class MemberDto
    {

        public int? Id { get; set; }
        public string MemAcc { get; set; }
        public int? Level { get; set; }
        public int? Suspension { get; set; }
        public int? ShopGold { get; set; }

        public DateTime CreateDate { get; set; }
    }
}