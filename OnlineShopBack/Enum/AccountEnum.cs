#region 功能與歷史修改描述
/*
    描述:後台帳號系統相關列舉
    日期:2022-05-05
*/
#endregion

namespace OnlineShopBack.Enum
{
    public class AccountEnum
    {

        #region 後台帳號相關列舉
        //新增帳號       
        public enum AddACCountErrorCode
        {
            //<summary >
            //帳號新增成功
            //</summary >
            AddOK = 0,

            //<summary >
            //帳號重複
            //</summary >
            duplicateAccount = 100,

            //<summary >
            //該權限未建立
            //</summary >
            permissionIsNull = 101
        }
        //編輯帳號
        public enum PutAccErrorCode 
        {
            //<summary >
            //帳號刪除成功
            //</summary >
            PutOK = 0,
            //<summary >
            //此帳號不可更改
            //</summary >
            ProhibitPut = 100,
            //<summary >
            //尚未建立權限
            //</summary >
            LvIsNull = 101

        }
        //修改密碼
        public enum PutAccPwdErrorCode 
        {
            //<summary >
            //密碼修改成功
            //</summary >
            PutOK = 0,
            //<summary >
            //新密碼與確認密碼不相同
            //</summary >
            confirmError = 100,
            //<summary >
            //尚未建立權限
            //</summary >
            AccIsNull = 101

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
