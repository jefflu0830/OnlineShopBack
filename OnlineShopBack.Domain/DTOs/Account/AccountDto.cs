using System;
using System.Data;

namespace OnlineShopBack.Domain.DTOs.Account
{
    public class AccountDto
    {
        //public AccountDto(DataRow row)
        //{
        //    if(row.Field<int>("aaa") == null)
        //    {
        //        this.Id = row["aaa"].ToString();
        //    }
        //}


        public int Id { get; set; } = -1;
        public string Account { get; set; }
        public string Pwd { get; set; }
        public int Level { get; set; }

        public int LevelName { get; set; }

        public DateTime CreateDate { get; set; }

        //public static (bool, AccountDto,DataRow) GrenerateInstance(DataRow row)
        public static (bool, AccountDto) GrenerateInstance(DataRow row)
        {
            AccountDto temp = new AccountDto();

            //if (row.Field<int>("aaa") == null)
            //{
            //    temp.Id = row["aaa"].ToString();
            //}
            //else
            //{
            //    return (false, null);
            //}
            temp.Id = (int)row[0];
            temp.Account = row[1].ToString();

            //if (row.Field<int>("aaa") == null)
            //{
            //    temp.Id = (int)row[""];
            //}
            //else
            //{
            //    return (false, null);
            //}

            return (true, temp);
        }
    }
}