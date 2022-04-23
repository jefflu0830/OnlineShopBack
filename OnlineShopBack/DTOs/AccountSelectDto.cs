using System;

namespace OnlineShopBack.Controllers
{
    public class AccountSelectDto
    {

        public int Id { get; set; }
        public string Account { get; set; }
        public string Pwd { get; set; }
        public int Level { get; set; }

        public DateTime CreateDate { get; set; }
    }
}