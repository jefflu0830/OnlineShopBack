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
