using System;

namespace OnlineShopBack.Controllers
{
    public class AccountLevelDto
    {

        public int? accLevel { get; set; }
        public string accPosition { get; set; }
        public int? canUseAccount { get; set; }
        public int? canUseMember { get; set; }

    }
}