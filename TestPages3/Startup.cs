using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TestPages3.Controllers;
using TestPages3.Models;

namespace TestPages3
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private Dictionary<string,string> config2;
        
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            config2 = new Dictionary<string, string>()
            {
                { "username", "admin" },
                { "password", "qwerty" },
                { "color", "#0000ff" },
                { "font-size", "34px" },
            };
            string connection = Configuration.GetConnectionString("DefaultConnection");
            
            

            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connection = Configuration.GetConnectionString("DefaultConnection");
            
            //Базы данных
            services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(connection)); //Телефонная книга
            services.AddDbContext<AccoutContext>(options =>
                options.UseSqlServer(connection)); // Аккаунты пользователей

            // установка конфигурации подключения
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => //CookieAuthenticationOptions
                {
                    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/AccessDenied"); ///Account/Login
                    options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Account/AccessDenied");
                });
            services.AddControllersWithViews();
            
            //Сервис времени существования
            services.AddTransient<UpTimeServiceSeconds>();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UpTimeServiceSeconds time)
        {
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
            
            app.UseAuthentication();    // аутентификация
            app.UseAuthorization();     // авторизация

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
        public string Message(string message, string color, string fontsize)
        {
            return $"<p style='color:{color}; font-size:{fontsize};'>{message}</p>";
        }
    }
}
