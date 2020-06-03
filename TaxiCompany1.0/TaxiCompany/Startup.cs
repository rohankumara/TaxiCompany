using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaxiCompany.Data;
using TaxiCompany.Models;
using TaxiCompany.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using TaxiCompany.Authorization;

namespace TaxiCompany
{
    public class Startup
    {
        public IHostingEnvironment _hostinEnv;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath).
                AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).
                AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            _hostinEnv = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();

            var skipSSL = Configuration.GetValue<bool>("LocalTest:skipSSL");

            services.Configure<MvcOptions>(options =>
            {
                // Set LocalTest:skipSSL to true to skip SSL requrement in 
                // debug mode. This is useful when not using Visual Studio.
                if (_hostinEnv.IsDevelopment() && !skipSSL)
                {
                    options.Filters.Add(new RequireHttpsAttribute());
                }
            });

            //Add authenticaion settings..
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 6;

                //Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(20);
                options.Lockout.MaxFailedAccessAttempts = 6;
                options.Lockout.AllowedForNewUsers = true;

                //User Settings
                options.User.RequireUniqueEmail = true;

            });

            services.ConfigureApplicationCookie(options =>
            {
                //Cockie settings.
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/SignedOut";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            }
            );

            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            //Authoriztion handlers
            services.AddScoped<IAuthorizationHandler, CustomerOwnerAuthorizationHandlers>();
            services.AddScoped<IAuthorizationHandler, DriverOwnerAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, AdminAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, AdminDriverAuthorizationHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvcWithDefaultRoute();

            //var testUserPw = Configuration["SeedUserPW"];

            //if (String.IsNullOrEmpty(testUserPw))
            //{
            //   throw new System.Exception("Use secrets manager to set SeedUserPW \n" +

            //                               "dotnet user-secrets set SeedUserPW <pw>");
            //}

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});


        }
    }
}
