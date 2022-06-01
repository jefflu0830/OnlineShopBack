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


            //CORS
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    //policy.WithOrigins("https://blog.johnwu.cc")
                    policy.WithOrigins("https://tw.yahoo.com")
                          .WithHeaders()
                          .WithMethods()
                          .AllowCredentials();
                });
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("CorsPolicy");

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
