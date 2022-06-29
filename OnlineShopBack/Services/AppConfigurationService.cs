#region 功能與歷史修改描述
/*
    描述:appsettings.json取得SQL 連線字串
    日期:2022-05-05
*/
#endregion
using Microsoft.Extensions.Configuration;
using System.IO;

namespace OnlineShopBack.Services
{
    public class AppConfigurationService
    {
        //private static readonly IConfigurationRoot configRoot;
        //private static readonly IConfigurationRoot someSettingRoot;
        //public static readonly string SQLConnectionString ;

        //static AppConfigurationService()
        //{
        //    configRoot = new ConfigurationBuilder().AddJsonFile(Directory.GetCurrentDirectory() + "/appsettings.json", optional: false, reloadOnChange: true).Build();
        //    //someSettingRoot = new ConfigurationBuilder().AddJsonFile(Directory.GetCurrentDirectory() + "/SomeSetting.json", optional: false, reloadOnChange: true).Build();
        //    SQLstring SQLConnectionStr = new SQLstring();
        //    configRoot.GetSection("ConnectionStrings").Bind(SQLConnectionStr);

        //    SQLConnectionString = SQLConnectionStr.OnlineShopDatabase;
        //}

        public static string GetConnectionStr()
        {            
            IConfigurationBuilder Builder = new ConfigurationBuilder();

            Builder.AddJsonFile(System.AppDomain.CurrentDomain.BaseDirectory + "/appsettings.json", optional: false, reloadOnChange: true);
            var configurationRoot = Builder.Build();
            SQLstring SQLConnectionStr = new SQLstring();
            //程式預設目錄
            //develop enviverment  // development environment 
            //net core development environment appseting  :1.https://marcus116.blogspot.com/2019/04/netcore-aspnet-core-appsettingsjson.html

            configurationRoot.GetSection("ConnectionStrings").Bind(SQLConnectionStr);            
           
            return SQLConnectionStr.OnlineShopDatabase;
        }

        class SQLstring
        {
            public string OnlineShopDatabase { get; set; }
        }

        
    }
}
