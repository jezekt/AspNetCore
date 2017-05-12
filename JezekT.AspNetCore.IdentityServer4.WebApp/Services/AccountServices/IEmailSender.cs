using System.Threading.Tasks;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Services.AccountServices
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
