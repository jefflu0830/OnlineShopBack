using System;

namespace OnlineShopBack.Controllers
{
    public class PutPwdDto
    {
        public int id { get; set; }
        public string newPwd { get; set; }
        public string cfmNewPwd { get; set; }
    }
}