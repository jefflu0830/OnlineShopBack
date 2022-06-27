using System.Data;

namespace OnlineShopBack.Domain.DTOs.Member
{
    public class MemLvDto
    {

        public int? memLv { get; set; }
        public string LvName { get; set; }

        public static (bool, MemLvDto) GetMemLvList(DataRow row)
        {
            MemLvDto temp = new MemLvDto();

            temp.memLv = (int)row["f_memberLevel"];
            temp.LvName = row["f_LevelName"].ToString();

            return (true, temp);
        }

    }
}