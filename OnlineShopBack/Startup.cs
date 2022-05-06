using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OnlineShopBack.Models;
using System;
using System.Threading.Tasks;
using OnlineShopBack.Domain;
using OnlineShopBack.Domain.Repository;
using OnlineShopBack.Persistent;

namespace OnlineShopBack
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IAccountRepository, AccountRepository>();

            services.AddControllersWithViews();
            services.AddControllers();
            services.AddDbContext<OnlineShopContext>(options =>
                                                     options.UseSqlServer(Configuration.GetConnectionString("OnlineShopDatabase")));


            ////Cookie 驗證
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
            {
                //未登入時會自動導到這個網址
                option.LoginPath = new PathString("/login");
                //沒有權限時會自動導到這個網址
                option.AccessDeniedPath = new PathString("/index");
                //設定時間失效
                //option.ExpireTimeSpan = TimeSpan.FromHours(5);
                option.ExpireTimeSpan = TimeSpan.FromSeconds(3000);
            });
            services.AddAuthentication();

            //全域套用 [Authorize]?
            services.AddMvc(options =>
            {
                options.Filters.Add(new AuthorizeFilter());
            });

            //session設定
            services.AddSession(o =>
            {
                o.IdleTimeout = TimeSpan.FromSeconds(1800);
            });

            services.AddMvc().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true; //tuer後 不管有沒有報錯 都先會進 controller
            });
            //-------------------------------------------------------------------------------
            //services.AddRazorPages(options =>
            //{
            //    options.Conventions.AuthorizeFolder("/Pages/Account","Pages");
            //});
            services.AddRazorPages();

        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            //Cookie驗證用  順序要一樣
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();//啟用session

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
