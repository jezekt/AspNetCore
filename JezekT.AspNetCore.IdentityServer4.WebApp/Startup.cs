using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4.EntityFramework.Entities;
using JezekT.AspNetCore.Bootstrap.Datepicker.Settings;
using JezekT.AspNetCore.DataTables.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using JezekT.AspNetCore.IdentityServer4.WebApp.Data;
using JezekT.AspNetCore.IdentityServer4.WebApp.Services.AccountServices;
using JezekT.AspNetCore.IdentityServer4.WebApp.Services.ClientServices;
using JezekT.AspNetCore.IdentityServer4.WebApp.Services.IdentityResourceServices;
using JezekT.AspNetCore.IdentityServer4.WebApp.Services.RoleServices;
using JezekT.AspNetCore.IdentityServer4.WebApp.Services.UserClaimServices;
using JezekT.AspNetCore.IdentityServer4.WebApp.Services.UserServices;
using JezekT.AspNetCore.Select2.Settings;
using JezekT.NetStandard.Pagination.DataProviders;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;

namespace JezekT.AspNetCore.IdentityServer4.WebApp
{
    public class Startup
    {
        private readonly IConfigurationRoot _configuration;


        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddDbContext<IdentityServerDbContext>(options => options.UseSqlServer(connectionString));
            services.AddIdentity<User, IdentityRole>(option => option.SecurityStampValidationInterval = TimeSpan.FromSeconds(30))
                .AddEntityFrameworkStores<IdentityServerDbContext>()
                .AddDefaultTokenProviders();
                //.AddTokenProvider(TokenOptions.DefaultEmailProvider, typeof(EmailTokenProvider<User>));

            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            var signingCert = new X509Certificate2(Path.Combine("c:/users/jezek/desktop", "identity.local.signing.pfx"), "best");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddIdentityServer()
                .AddSigningCredential(signingCert)
                .AddConfigurationStore(builder => builder.UseSqlServer(connectionString, options => options.MigrationsAssembly(migrationsAssembly)))
                .AddOperationalStore(builder => builder.UseSqlServer(connectionString, options => options.MigrationsAssembly(migrationsAssembly)))
                .AddAspNetIdentity<User>();

            services.AddTransient<IPaginationDataProvider<User, object>, UserPaginationProvider>();
            services.AddTransient<IPaginationDataProvider<IdentityUserClaim<string>, object>, UserClaimPaginationProvider>();
            services.AddTransient<IPaginationDataProvider<IdentityRole, object>, RolePaginationProvider>();
            services.AddTransient<IPaginationDataProvider<Client, object>, ClientPaginationProvider>();
            services.AddTransient<IPaginationDataProvider<IdentityResource, object>, IdentityResourcePaginationProvider>();

            services.AddTransient<IPasswordResetSender, AccountEmailTools>();
            services.AddTransient<IEmailConfirmationSender, AccountEmailTools>();
            services.AddTransient<IEmailSender, EmailSender>();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("cs-CZ"),
                };

                options.DefaultRequestCulture = new RequestCulture(supportedCultures.First());
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdministratorOnly", policy => policy.RequireRole("Administrator"));
                options.AddPolicy("UserAdministratorOnly", policy => policy.RequireRole("Administrator", "UserAdministrator"));
            });

            DataTableSettings.LocalizationUrl = Resources.Services.DataTable.DataTableSettings.LocalizationUrl;
            SelectDropdownSettings.LanguageCode = Resources.Services.Select2.SelectDropdownSettings.LanguageCode;
            SelectDropdownSettings.Loading = Resources.Services.Select2.SelectDropdownSettings.Loading;
            SelectDropdownSettings.LocalizationUrl = Resources.Services.Select2.SelectDropdownSettings.LocalizationUrl;
            DatepickerSettings.LanguageCode = Resources.Services.Datepicker.DatepickerSettings.LanguageCode;
            DatepickerSettings.LocalizationUrl = Resources.Services.Datepicker.DatepickerSettings.LocalizationUrl;

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            InitializeDatabase(app);

            loggerFactory.AddConsole(LogLevel.Debug);
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();
            app.UseIdentityServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }


        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath).AddJsonFile("appsettings.json", false, true);
            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            _configuration = builder.Build();
        }


        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<IdentityServerDbContext>();
                var adminUsername = "admin";
                var defaultPassword = "Admin123||";
                var adminRoleValue = "Administrator";
                
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                if (!roleManager.RoleExistsAsync(adminRoleValue).Result)
                {
                    var adminRole = new IdentityRole(adminRoleValue);
                    roleManager.CreateAsync(adminRole).GetAwaiter().GetResult();
                }

                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var admin = context.Users.FirstOrDefault(x => x.UserName == adminUsername);
                if (admin == null)
                {
                    admin = new User {UserName = adminUsername};
                    userManager.CreateAsync(admin, defaultPassword).GetAwaiter().GetResult();
                    userManager.AddToRoleAsync(admin, adminRoleValue).GetAwaiter().GetResult();
                }
                else
                {
                    if (!userManager.IsInRoleAsync(admin, adminRoleValue).GetAwaiter().GetResult())
                    {
                        userManager.AddToRoleAsync(admin, adminRoleValue).GetAwaiter().GetResult();
                    }
                }
            }
        }
    }
}
