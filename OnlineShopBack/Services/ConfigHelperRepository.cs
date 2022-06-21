using Microsoft.Extensions.Configuration;
using OnlineShopBack.Domain.Repository;

namespace OnlineShopBack.Services
{
    public class ConfigHelperRepository : IConfigHelperRepository
    {
        
        public string SQLConnectionStrings()
        {
            string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");

            return SQLConnectionString;
        }
    }
}
