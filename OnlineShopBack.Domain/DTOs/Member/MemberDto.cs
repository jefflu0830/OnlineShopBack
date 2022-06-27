using System;
using System.Data;

namespace OnlineShopBack.Domain.DTOs.Member

{
    public class MemberDto
    {
        public int? Id { get; set; }
        public string MemAcc { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Mail { get; set; }
        public int? Level { get; set; }
        public string LevelName { get; set; }
        public int? Suspension { get; set; }
        public string SuspensionName { get; set; }
        public int? ShopGold { get; set; }
        public string CreateDate { get; set; }

        public static (bool, MemberDto) GetMemberList(DataRow row)
        {
            MemberDto temp = new MemberDto();

            temp.Id = (int)row["f_id"];
            temp.MemAcc= row["f_acc"].ToString();
            temp.Name = row["f_name"].ToString();
            temp.Phone = row["f_phone"].ToString();
            temp.Address = row["f_address"].ToString();
            temp.Mail = row["f_mail"].ToString();
            temp.ShopGold = (int)row["f_shopGold"];
            temp.LevelName = row["f_LevelName"].ToString();
            temp.SuspensionName = row["f_suspensionName"].ToString();
            temp.CreateDate = row["f_createDate"].ToString();
            //if (row.Field<int?>("f_accLevel") != null &&
            //    row.Field<string>("f_accPosition") != null)
            //{
            //    temp.Id = (int)row["f_accLevel"];                
            //}
            //else
            //{
            //    return (false, null);
            //}

            return (true, temp);
        }

        public static (bool, MemberDto) GetMemberListByAcc(DataRow row)
        {
            MemberDto temp = new MemberDto();

            temp.Id = (int)row["f_id"];
            temp.MemAcc = row["f_acc"].ToString();
            temp.Name = row["f_name"].ToString();
            temp.Mail = row["f_mail"].ToString();
            temp.ShopGold = (int)row["f_shopGold"];

            return (true, temp);
        }
    }
}