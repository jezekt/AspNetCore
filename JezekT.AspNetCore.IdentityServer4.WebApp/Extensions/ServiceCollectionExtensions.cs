using JezekT.AspNetCore.IdentityServer4.WebApp.Services.AccountServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddEmailSender(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IEmailSender, EmailSender>();

            EmailSenderOptions.SenderName = configuration.GetValue<string>("EmailSender:SenderName");
            EmailSenderOptions.SenderEmail = configuration.GetValue<string>("EmailSender:SenderEmail");
            EmailSenderOptions.Host = configuration.GetValue<string>("EmailSender:Host");
            EmailSenderOptions.Port = configuration.GetValue<int>("EmailSender:Port");
        }
    }
}
