namespace OnlineShopBack.Domain.Repository
{
    public interface IAccountRepository
    {
        public (string, int)[] GetAccountAndLevelList();

        public (string, int)[] GetTargetAccount(string id);
    }
}
