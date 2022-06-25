using System;
using System.Data;

namespace OnlineShopBack.Domain.DTOs.Account
{
    public class AccountLevelDto
    {

        public int? accLevel { get; set; }
        public string accPosition { get; set; }
        public int? canUseAccount { get; set; }
        public int? canUseMember { get; set; }
        public int? canUseProduct { get; set; }
        public int? canUseOrder { get; set; }

        public static (bool, AccountLevelDto) GetAccLvList(DataRow row)
        {
            AccountLevelDto temp = new AccountLevelDto();

            if (row.Field<int?>("f_accLevel") != null &&
                row.Field<string>("f_accPosition") != null)
            {
                temp.accLevel = (int)row["f_accLevel"];
                temp.accPosition = row["f_accPosition"].ToString();
            }
            else
            {
                return (false, null);
            }

            return (true, temp);
        }

        public static (bool, AccountLevelDto) GetAccLvById(DataRow row)
        {
            AccountLevelDto temp = new AccountLevelDto();

            if (row.Field<int?>("f_accLevel") != null &&
                row.Field<string>("f_accPosition") != null)
            {
                temp.accLevel = (int)row["f_accLevel"];
                temp.accPosition = row["f_accPosition"].ToString();
                temp.canUseAccount = Convert.ToInt32(Convert.ToBoolean(row["f_canUseAccount"]));
                temp.canUseMember = Convert.ToInt32(Convert.ToBoolean(row["f_canUseMember"]));
                temp.canUseProduct = Convert.ToInt32(Convert.ToBoolean(row["f_canUseProduct"]));
                temp.canUseOrder = Convert.ToInt32(Convert.ToBoolean(row["f_canUseOrder"]));
            }
            else
            {
                return (false, null);
            }

            return (true, temp);
        }

    }
}