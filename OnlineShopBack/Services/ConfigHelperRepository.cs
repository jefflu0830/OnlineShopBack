using Microsoft.Extensions.Configuration;
using OnlineShopBack.Domain.Repository;

namespace OnlineShopBack.Services
{
    public class ConfigHelperRepository : IConfigHelperRepository
    {
        
        public string SQLConnectionStrings()
        {     
            string SQLConnectionString = AppConfigurationService.GetConnectionStr();
            return SQLConnectionString;
        }
    }
}
