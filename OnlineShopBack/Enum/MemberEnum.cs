#region 功能與歷史修改描述
/*
    描述:後台帳號系統相關列舉
    日期:2022-05-05
*/
#endregion

namespace OnlineShopBack.Enum
{
    public class MemberEnum
    {

        #region 會員相關列舉(Enum)
        //刪除會員
        public enum DelMemberErrorCode 
        {
            //<summary >
            //會員刪除成功
            //</summary >
            DelOK = 0,
            //<summary >
            //無此會員
            //</summary >
            MemIsNull = 100
        }

        //編輯會員
        public enum PutMemberErrorCode 
        {
            //<summary >
            //1編輯成功
            //</summary >
            PutOK = 0,
            //<summary >
            //無此會員
            //</summary >
            MemIsNull = 100,
            //<summary >
            //無此會員
            //</summary >
            LvIsNull = 101,
            //<summary >
            //無此會員
            //</summary >
            SuspensionNull = 102
        }

        //購物金調整
        public enum PutShopGoldCode 
        {
            //<summary >
            //調整成功
            //</summary >
            PutOK = 0,
            //<summary >
            //無此會員
            //</summary >
            MemIsNull = 100,
            //<summary >
            //原始購物金與帳號不相符
            //</summary >
            Incompatible = 101,
            //<summary >
            //調整後購物金不得小於0 or 大於20000
            //</summary >
            PutShopGoldError = 102
        }
        #endregion

        #region 會員等級相關列舉(Enum)
        public enum AddMemLvErrorCode //增加會員等級
        {
            //<summary >
            //添加成功
            //</summary >
            AddOK = 0,
            //<summary >
            //無此等級
            //</summary >
            DuplicateMemLv = 100
        }

        public enum DelMemLvErrorCode //刪除會員等級
        {
            //<summary >
            //會員刪除成功
            //</summary >
            DelOK = 0,
            //<summary >
            //此等級不可刪除
            //</summary >
            ProhibitDel = 100,
            //<summary >
            //此等級尚未建立
            //</summary >
            IsNull = 101,
            //<summary >
            //有使用者正在套用此等級
            //</summary >
            IsUsing = 102,
        }

        public enum PutMemLVErrorCode // 更新權限
        {
            //<summary >
            //權限更新成功
            //</summary >
            PutOK = 0,
            //<summary >
            //尚未建立此權限
            //</summary >
            LvIsNull = 100

        }

        #endregion

        #region 會員狀態相關列舉(Enum)
        //增加會員狀態
        public enum AddSuspensionCode 
        {
            //<summary >
            //會員刪除成功
            //</summary >
            AddOK = 0,
            //<summary >
            //無此會員
            //</summary >
            Duplicate = 100
        }
        //刪除會員狀態
        public enum DelSuspensionCode 
        {
            //<summary >
            //刪除成功
            //</summary >
            DelOK = 0,
            //<summary >
            //禁止刪除
            //</summary >
            ProhibitDel = 100,
            //<summary >
            //編號尚未建立
            //</summary >
            isNull = 101
        }
        // 更新狀態
        public enum PutSuspensionCode 
        {
            //<summary >
            //權限更新成功
            //</summary >
            PutOK = 0,
            //<summary >
            //尚未建立此權限
            //</summary >
            IsNull = 100

        }

        #endregion

    }
}
