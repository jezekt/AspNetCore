using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using JezekT.AspNetCore.IdentityServer4.WebApp.Data;
using JezekT.AspNetCore.IdentityServer4.WebApp.Extensions;
using JezekT.AspNetCore.IdentityServer4.WebApp.Services.AccountServices;
using JezekT.AspNetCore.IdentityServer4.WebApp.Services.ApiResourceServices;
using JezekT.AspNetCore.IdentityServer4.WebApp.Services.ClientServices;
using JezekT.AspNetCore.IdentityServer4.WebApp.Services.IdentityResourceServices;
using JezekT.AspNetCore.IdentityServer4.WebApp.Services.RoleServices;
using JezekT.AspNetCore.IdentityServer4.WebApp.Services.UserClaimServices;
using JezekT.AspNetCore.IdentityServer4.WebApp.Services.UserServices;
using JezekT.NetStandard.Pagination.DataProviders;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using Serilog;

namespace JezekT.AspNetCore.IdentityServer4.WebApp
{
    public class Startup
    {
        private readonly IConfigurationRoot _configuration;


        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<IdentityServerDbContext>(options => options.UseSqlServer(connectionString));
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<IdentityServerDbContext>()
                .AddDefaultTokenProviders();

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("cs-CZ"),
                    new CultureInfo("en")
                };

                options.DefaultRequestCulture = new RequestCulture(supportedCultures.First());
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.AddMvc(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()))
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            services.AddTransient<IPaginationDataProvider<User, object>, UserPaginationProvider>();
            services.AddTransient<IPaginationDataProvider<IdentityUserClaim<string>, object>, UserClaimPaginationProvider>();
            services.AddTransient<IPaginationDataProvider<IdentityRole, object>, RolePaginationProvider>();
            services.AddTransient<IPaginationDataProvider<Client, object>, ClientPaginationProvider>();
            services.AddTransient<IPaginationDataProvider<IdentityResource, object>, IdentityResourcePaginationProvider>();
            services.AddTransient<IPaginationDataProvider<ApiResource, object>, ApiResourcePaginationProvider>();
            services.AddTransient<IPasswordResetSender, AccountEmailTools>();
            services.AddTransient<IEmailConfirmationSender, AccountEmailTools>();
            services.AddEmailSender(_configuration);

            services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromSeconds(_configuration.GetValue<int>("SecurityStampValidationInterval"));
            });

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var identityServerBuilder = services.AddIdentityServer()
                .AddConfigurationStore(options =>
                    options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString,
                        sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly)))
                .AddOperationalStore(options =>
                    options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString,
                        sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly)))
                .AddAspNetIdentity<User>();

            if (_configuration.GetValue<bool>("SigningCertificate:UseDeveloperSigningCredentials"))
            {
                identityServerBuilder.AddDeveloperSigningCredential();
            }
            else
            {
                var signingCert = new X509Certificate2(_configuration.GetValue<string>("SigningCertificate:Path"), _configuration.GetValue<string>("SigningCertificate:Password"));
                identityServerBuilder.AddSigningCredential(signingCert);
            }

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdministratorOnly", policy => policy.RequireRole("Administrator"));
                options.AddPolicy("UserAdministratorOnly", policy => policy.RequireRole("Administrator", "UserAdministrator"));
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            InitializeDatabase(app);

            if (env.IsDevelopment())
            {
                loggerFactory.AddConsole(_configuration.GetSection("Logging"));
                loggerFactory.AddDebug();
                loggerFactory.AddSerilog(new LoggerConfiguration().ReadFrom.Configuration(_configuration)
                    .WriteTo.Async(a => new LoggerConfiguration().ReadFrom.Configuration(_configuration), 500)
                    .CreateLogger(), true);

                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                loggerFactory.AddSerilog(new LoggerConfiguration().ReadFrom.Configuration(_configuration)
                    .WriteTo.Async(a => new LoggerConfiguration().ReadFrom.Configuration(_configuration), 500)
                    .CreateLogger(), true);

                app.UseStatusCodePages();
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseRequestLocalization(app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>().Value);
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
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);

            builder.AddEnvironmentVariables();
            _configuration = builder.Build();
        }


        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<IdentityServerDbContext>().Database.Migrate();
                serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>().Database.Migrate();
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<IdentityServerDbContext>();
                var adminUsername = _configuration.GetValue<string>("DefaultAdministrator:Username");
                var defaultPassword = _configuration.GetValue<string>("DefaultAdministrator:Password");
                var adminRoleValue = _configuration.GetValue<string>("DefaultAdministrator:Role");
                
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
