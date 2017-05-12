using System.Threading.Tasks;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Services.AccountServices
{
    public interface IEmailConfirmationSender
    {
        Task SendEmailConfirmationAsync(string email, string callbackUrl);
    }
}
