using System.Threading.Tasks;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Services.AccountServices
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
