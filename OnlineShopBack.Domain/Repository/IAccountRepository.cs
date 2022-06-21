using System.Data;
using OnlineShopBack.Domain.DTOs.Account;

namespace OnlineShopBack.Domain.Repository
{
    public interface IAccountRepository
    {
        //-----------後台帳號相關-----------

        public DataTable GetAccountAndLevelList();//後臺帳號清單

        public int AddAccount( AccountDto value);//新增後臺帳號

        public int EditAcc( int id, AccountDto value);//後臺帳號編輯權限

        public int EditPwd( PutPwdDto value);//編輯後臺帳號密碼

        public int DelAcc( int id);//刪除帳號

        //-----------後台帳號權限相關-----------
        public DataTable GetAccLvList();//後臺帳號清單

        public DataTable GetAccLvById(int id);//依照後臺帳號id取得該後台帳號資料

        public int AddAccLv(AccountLevelDto value);//新增後臺帳號

        public int EditAccLv(int id, AccountLevelDto value);//後臺帳號權限編輯

        public int DelAccLv(int id);//後台帳號權限刪除
    }
}
