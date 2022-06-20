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
