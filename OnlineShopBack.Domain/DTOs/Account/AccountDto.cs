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


        public int Id { get; set; }
        public string Account { get; set; }
        public string Pwd { get; set; }
        public int Level { get; set; }

        public string LevelName { get; set; }

        public string CreateDate { get; set; }

        public static (bool, AccountDto) GrenerateInstance(DataRow row)
        {
            AccountDto temp = new AccountDto();

            if (row.Field<int?>("f_id") != null &&
                row.Field<string>("f_acc") != null &&
                row.Field<string>("f_accPosition") != null &&
                row.Field<DateTime>("f_createDate") != null)

            {
                temp.Id = (int)row["f_id"];
                temp.Account = row["f_acc"].ToString();
                temp.LevelName = row["f_accPosition"].ToString();
                temp.CreateDate = row["f_createDate"].ToString();
            }
            else
            {
                return (false, null);
            }

            return (true, temp);
        }
    }
}