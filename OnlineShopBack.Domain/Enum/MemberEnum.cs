#region 功能與歷史修改描述
/*
    描述:後台帳號系統相關列舉
    日期:2022-05-05
*/
#endregion

namespace OnlineShopBack.Domain.Enum
{
    public class MemberEnum
    {

        #region 會員相關列舉(Enum)
        //刪除會員
        public enum DelMemberCode 
        {
            //<summary >
            //會員刪除成功
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

        //編輯會員
        public enum PutMemberCode 
        {
            //<summary >
            //1編輯成功
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

        //購物金調整
        public enum EditShopGoldCode 
        {
            //<summary >
            //調整成功
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
        #endregion

        #region 會員等級相關列舉(Enum)
        public enum AddMemLvCode //增加會員等級
        {
            //<summary >
            //添加成功
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
        public enum PutMemLVCode // 更新權限
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

        public enum DelMemLvCode //刪除會員等級
        {
            //<summary >
            //會員刪除成功
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

        #region 會員狀態相關列舉(Enum)
        //增加會員狀態
        public enum AddSuspensionCode 
        {
            //<summary >
            //會員刪除成功
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
        // 更新狀態
        public enum EditSuspensionCode
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
        //刪除會員狀態
        public enum DelSuspensionCode 
        {
            //<summary >
            //刪除成功
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
