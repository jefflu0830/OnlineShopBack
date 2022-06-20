using System.Data;
using OnlineShopBack.Domain.DTOs.Account;

namespace OnlineShopBack.Domain.Repository
{
    public interface IAccountRepository
    {
        //-----------後台帳號相關-----------
        public DataTable GetAccountAndLevelList(string SQLConnectionString);//後臺帳號清單

        public int AddAccount(string SQLConnectionString, AccountDto value);//新增後臺帳號

        public int EditAcc(string SQLConnectionString, int id, AccountDto value);//後臺帳號編輯權限

        public int EditPwd(string SQLConnectionString, PutPwdDto value);//編輯後臺帳號密碼

        public int DelAcc(string SQLConnectionString, int id);//刪除帳號

        //-----------後台帳號權限相關-----------
        public DataTable GetAccLvList(string SQLConnectionString);//後臺帳號清單

        public DataTable GetAccLvById(string SQLConnectionString, int id);//依照後臺帳號id取得該後台帳號資料
    }
}
