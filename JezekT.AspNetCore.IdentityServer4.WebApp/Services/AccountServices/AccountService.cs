using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using IdentityServer4.Services;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.AccountViewModels;
using Microsoft.AspNetCore.Http;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Services.AccountServices
{
    public class AccountService
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            return new LoginViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint
            };
        }

        public async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            //var user = await _httpContextAccessor.HttpContext.GetIdentityServerUserAsync();
            if (_httpContextAccessor.HttpContext.User == null || _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated == false)
            {
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            return vm;
        }

        public async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = logout?.ClientId,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };
            return vm;
        }


        public AccountService(IIdentityServerInteractionService interaction, IHttpContextAccessor httpContextAccessor)
        {
            if (interaction == null || httpContextAccessor == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            _interaction = interaction;
            _httpContextAccessor = httpContextAccessor;
        }
        
    }
}
