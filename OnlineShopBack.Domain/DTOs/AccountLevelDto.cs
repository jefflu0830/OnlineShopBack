namespace OnlineShopBack.Domain.DTOS
{
    public class AccountLevelDto
    {

        public int? accLevel { get; set; }
        public string accPosition { get; set; }
        public int? canUseAccount { get; set; }
        public int? canUseMember { get; set; }

    }
}