using System;

namespace OnlineShopBack.Domain.DTOs.Account
{
    public class AccountDto
    {

        public int Id { get; set; }
        public string Account { get; set; }
        public string Pwd { get; set; }
        public int Level { get; set; }

        public DateTime CreateDate { get; set; }
    }
}