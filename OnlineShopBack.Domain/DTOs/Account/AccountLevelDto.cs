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

    }
}