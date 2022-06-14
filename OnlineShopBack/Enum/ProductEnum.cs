#region 功能與歷史修改描述
/*
    描述:後台商品系統相關列舉
    日期:2022-05-05
*/
#endregion
namespace OnlineShopBack.Enum
{
    public class ProductEnum
    {
        //商品相關列舉(Enum)        
        public enum ProductReturnCode //新增商品
        {
            //<summary >
            //成功
            //</summary >
            Success = 0,
            //<summary >
            //失敗
            //</summary >
            Fail = 1,
            //<summary >
            //預設值
            //</summary >
            Default = 2,
            //<summary >
            //圖片格式錯誤
            //</summary >
            ImgFormatErr = 3,
            //<summary >
            //圖片不可空白且只能上傳一張
            //</summary >
            ImgFail = 4
        }

        public enum CategoryReturnCode//[類別]相關代號
        {
            //<summary >
            //成功
            //</summary >
            Success = 0,
            //<summary >
            //失敗
            //</summary >
            Fail = 1,
            //<summary >
            //預設值
            //</summary >
            Default = 2
        }
    }
}
