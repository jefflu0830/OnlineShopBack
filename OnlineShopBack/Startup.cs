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

            //EntityFramework  Ҫʹ�ð�MODEL�ӻ��Ŀ
            //services.AddDbContext<OnlineShopContext>(options =>
            //                                         options.UseSqlServer(Configuration.GetConnectionString("OnlineShopDatabase")));

            services.AddAuthentication();

            //session�O��
            services.AddSession(o =>
            {
                //session���ʧЧ
                o.IdleTimeout = TimeSpan.FromSeconds(1800);
            });

            services.AddMvc().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true; //true �� �����Л]�Ј��e ���ȕ��M controller
            });

            services.AddRazorPages();

            services.AddDistributedMemoryCache();

            //��һֱͨ�^CORS��ԭ������� �зֺ���Ո��(GET , POST) and �A�zՈ��(PUT , DELETE)
            //����Ո��Ͳ������^����z��
            //GET ��POST Ҫ���k��׌�����^�A�z  ������ Content-Type  ���^ֵheader
            //https://developer.mozilla.org/zh-TW/docs/Web/HTTP/CORS

            /*
            JS �yԇ
            var request = new XMLHttpRequest();

            request.open('POST', 'https://localhost:5001/api/Order/AddTransport', true);
            request.setRequestHeader('Content-Type', 'application/json');

            request.send({ });*/

            //�A�OCORS
            //�·�Configure �е� app.useCors(),�Ͳ�횼������Q
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

            //addCors���������econtroller�ķ��o
            //��controller�� ���Ϙ˻` [EnableCors("CorsPolicy")]
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

            app.UseSession();//����session

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

            //Cookie��C��  ���Ҫһ��
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
