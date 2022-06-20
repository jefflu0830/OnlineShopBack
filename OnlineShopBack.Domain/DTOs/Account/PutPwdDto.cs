namespace OnlineShopBack.Domain.DTOs.Account
{
    public class PutPwdDto
    {
        public int id { get; set; }
        public string newPwd { get; set; }
        public string cfmNewPwd { get; set; }
    }
}