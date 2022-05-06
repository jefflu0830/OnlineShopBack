namespace OnlineShopBack.Domain.Enum
{
    public enum addACCountErrorCode //新增帳號
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
}
