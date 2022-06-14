#region 功能與歷史修改描述
/*
    描述:appsettings.json取得SQL 連線字串
    日期:2022-05-05
*/
#endregion
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace OnlineShopBack.Services
{
    public class AppConfigurationService
    {
        public static IConfiguration Configuration { get; set; }
        static AppConfigurationService()
        {
            Configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true }).Build();
        }
    }
}
