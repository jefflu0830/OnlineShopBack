using System.Data;

namespace OnlineShopBack.Domain.DTOs.Member
{
    public class suspensionDto
    {

        public int? suspensionLv { get; set; }
        public string suspensionName { get; set; }

        public static (bool, suspensionDto) GetSuspensionList(DataRow row)
        {
            suspensionDto temp = new suspensionDto();

            temp.suspensionLv = (int)row["f_suspensionLv"];
            temp.suspensionName = row["f_suspensionName"].ToString();

            return (true, temp);
        }

    }
}