#region 功能與歷史修改描述
/*
    描述:後台帳號系統相關列舉
    日期:2022-05-05
*/
#endregion

namespace OnlineShopBack.Domain.Enum
{
    public class AccountEnum
    {

        #region 後台帳號相關列舉
        //新增帳號       
        public enum AddAccountCode
        {
            //<summary >
            //帳號新增成功
            //</summary >
            AddOK = 0,
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
        //編輯帳號
        public enum EditAccCode
        {
            //<summary >
            //帳號刪除成功
            //</summary >
            PutOK = 0,
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
        //修改密碼
        public enum EditAccPwdCode 
        {
            //<summary >
            //密碼修改成功
            //</summary >
            PutOK = 0,
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
        //刪除帳號
        public enum DelAccCode
        {
            //<summary >
            //帳號刪除成功
            //</summary >
            DelOK = 0,
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
        #endregion

        #region 後台帳號權限相關列舉
        //新增權限
        public enum AddAccountLVCode 
        {
            //<summary >
            //權限新增成功
            //</summary >
            addOK = 0,
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
        // 更新權限
        public enum EditAccLvCode
        {
            //<summary >
            //權限更新成功
            //</summary >
            PutOK = 0,
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
        //刪除權限
        public enum DelAccLVCode 
        {
            //<summary >
            //權限刪除成功
            //</summary >
            DelOK = 0,
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


        #endregion

    }
}
