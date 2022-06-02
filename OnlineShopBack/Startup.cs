using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace OnlineShopBack
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }



        public void ConfigureServices(IServiceCollection services)
        {


            services.AddHttpContextAccessor();

            services.AddControllersWithViews();
            services.AddControllers();

            //EntityFramework  要使用把MODEL加回項目
            //services.AddDbContext<OnlineShopContext>(options =>
            //                                         options.UseSqlServer(Configuration.GetConnectionString("OnlineShopDatabase")));

            services.AddAuthentication();

            //session設定
            services.AddSession(o =>
            {
                //session多久失效
                o.IdleTimeout = TimeSpan.FromSeconds(1800);
            });

            services.AddMvc().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true; //true 後 不管有沒有報錯 都先會進 controller
            });

            services.AddRazorPages();

            services.AddDistributedMemoryCache();

            //會一直通過CORS的原因是因為 有分簡單請求(GET , POST) and 預檢請求(PUT , DELETE)
            //簡單請求就不會經過跨域檢查
            //GET 跟POST 要想辦法讓他經過預檢  可利用 Content-Type  標頭值header
            //https://developer.mozilla.org/zh-TW/docs/Web/HTTP/CORS

            /*
            JS 測試
            var request = new XMLHttpRequest();

            request.open('POST', 'https://localhost:5001/api/Order/AddTransport', true);
            request.setRequestHeader('Content-Type', 'application/json');

            request.send({ });*/

            //預設CORS
            //下方Configure 中的 app.useCors(),就不須加上名稱
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("https://blog.johnwu.cc")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            //addCors可以做個別controller的防護
            //在controller上 加上標籤 [EnableCors("CorsPolicy")]
            //https://stackoverflow.com/questions/31942037/how-to-enable-cors-in-asp-net-core
            //CORS
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("CorsPolicy", policy =>
            //    {
            //        //policy.WithOrigins("https://blog.johnwu.cc")
            //        policy.WithOrigins("https://tw.yahoo.com")
            //              .WithHeaders()
            //              .WithMethods()
            //              .AllowCredentials();
            //    });
            //});

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseCors("CorsPolicy");
            app.UseCors();

            app.UseSession();//啟用session

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            //Cookie驗證用  順序要一樣
            //app.UseCookiePolicy();
            //app.UseAuthentication();
            //app.UseAuthorization();



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
            });
        }
    }

}
