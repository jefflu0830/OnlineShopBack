using System;

namespace OnlineShopBack.Controllers
{
    public class PutPwdDto
    {

        public string Acc { get; set; }
        public string NewPwd { get; set; }
        public string OldPwd { get; set; }

    }
}