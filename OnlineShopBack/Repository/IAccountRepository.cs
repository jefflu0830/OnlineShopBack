using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace OnlineShopBack.Domain.Repository
{
    public interface IAccountRepository
    {
        public DataTable GetAccountAndLevelList(string SQLConnectionString);
    }
}
