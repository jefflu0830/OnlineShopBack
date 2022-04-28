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
            

            services.AddControllersWithViews();
            services.AddControllers();
            services.AddDbContext<OnlineShopContext>(options =>
                                                     options.UseSqlServer(Configuration.GetConnectionString("OnlineShopDatabase")));


            //Cookie ��C
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
            {
                //δ����r���Ԅӌ����@���Wַ
                option.LoginPath = new PathString("/Index");
                //�]�Й��ޕr���Ԅӌ����@���Wַ
                option.AccessDeniedPath = new PathString("/BackPage");
                //�O���r�gʧЧ
                //option.ExpireTimeSpan = TimeSpan.FromHours(5);
                option.ExpireTimeSpan = TimeSpan.FromSeconds(3000);
            });
            //ȫ������ [Authorize]?
            services.AddMvc(options =>
            {
                options.Filters.Add(new AuthorizeFilter());
            });


            services.AddMvc().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true; //tuer�� �����Л]�Ј��e ���ȕ��M controller
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

            //Cookie��C��  ���Ҫһ��
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}") ;

            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
