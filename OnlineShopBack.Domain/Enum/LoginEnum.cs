#region 功能與歷史修改描述
/*
    描述:登入系統相關
    日期:2022-05-05
*/
#endregion

namespace OnlineShopBack.Enum
{
    public class LoginEnum
    {
        //登入功能列舉(Enum)        
        public enum LoginReturnCode 
        {
            //<summary >
            //成功
            //</summary >
            Success = 0,
            //<summary >
            //登入失敗 帳號or密碼錯誤
            //</summary >
            AccOrPwdError = 100,
            //<summary >
            //後端驗證失敗
            //</summary >
            ValidaFail = 200,
            //<summary >
            //例外錯誤
            //</summary >
            ExceptionError = 201,
            //<summary >
            //Defult
            //</summary >
            Defult = 999
        }
    }
}
