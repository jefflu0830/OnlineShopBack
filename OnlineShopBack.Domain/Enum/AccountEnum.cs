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
        public enum DelACCountErrorCode
        {
            //<summary >
            //帳號刪除成功
            //</summary >
            DelOK = 0,
            //<summary >
            //此帳號不可刪除
            //</summary >
            ProhibitDel = 100,
            //<summary >
            //無此帳號
            //</summary >
            AccIsNull = 101

        }
        #endregion

        #region 後台帳號權限相關列舉
        //新增權限
        public enum addACCountLVErrorCode 
        {
            //<summary >
            //權限新增成功
            //</summary >
            addOK = 0,
            //<summary >
            //權限重複
            //</summary >
            duplicateAccountLv = 100
        }
        //刪除權限
        public enum DelACCountLVErrorCode 
        {
            //<summary >
            //權限刪除成功
            //</summary >
            DelOK = 0,
            //<summary >
            //有帳號使用此權限中
            //</summary >
            IsUsing = 100,
            //<summary >
            //尚未建立此權限
            //</summary >
            LvIsNull = 101
        }
        // 更新權限
        public enum PutACCountLVErrorCode 
        {
            //<summary >
            //權限更新成功
            //</summary >
            PutOK = 0,
            //<summary >
            //超級使用者不可更改
            //</summary >
            prohibitPutlv = 100,
            //<summary >
            //尚未建立此權限
            //</summary >
            LvIsNull = 101

        }

        #endregion

    }
}
