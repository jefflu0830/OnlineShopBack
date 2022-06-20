using System.Data;
using OnlineShopBack.Domain.DTOs.Account;

namespace OnlineShopBack.Domain.Repository
{
    public interface IAccountRepository
    {
        public DataTable GetAccountAndLevelList(string SQLConnectionString);

        public int AddAccount(string SQLConnectionString, AccountDto value);

        public int EditAcc(string SQLConnectionString, int id, AccountDto value);

        public int EditPwd(string SQLConnectionString, PutPwdDto value);
    }
}
