#region 功能與歷史修改描述
/*
    描述:後台帳號系統相關
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
            //帳號驗證失敗
            //</summary >
            AccOrPwdError = 100,
            //<summary >
            //重複登入
            //</summary >
            RepeatLogin = 101,
            //<summary >
            //後端驗證錯誤
            //</summary >
            BackEndError = 102
        }
    }
}
