using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Services.AccountServices
{
    public class AccountEmailTools : IEmailConfirmationSender, IPasswordResetSender
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        

        public async Task SendEmailConfirmationAsync(string email, string callbackUrl)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(callbackUrl)) throw new ArgumentNullException();
            Contract.EndContractBlock();
            var linkMessage = string.Format(Resources.Services.Account.AccountEmailTools.ConfirmEmailAccountMessage, 
                $"<a href='{callbackUrl}'>{Resources.Services.Account.AccountEmailTools.Link}</a>");
            await _emailSender.SendEmailAsync(email, Resources.Services.Account.AccountEmailTools.ConfirmEmailAccountSubject, linkMessage);
            _logger.LogInformation(string.Format(Resources.Services.Account.AccountEmailTools.ConfirmationSentToX, email));
        }

        public async Task SendPasswordResetAsync(string email, string callbackUrl)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(callbackUrl)) throw new ArgumentNullException();
            Contract.EndContractBlock();

            var linkMessage = string.Format(Resources.Services.Account.AccountEmailTools.ResetPasswordMessage,
                $"<a href='{callbackUrl}'>{Resources.Services.Account.AccountEmailTools.Link}</a>");
            await _emailSender.SendEmailAsync(email, Resources.Services.Account.AccountEmailTools.ResetPasswordSubject, linkMessage);
            _logger.LogInformation(string.Format(Resources.Services.Account.AccountEmailTools.PasswordResetSentToX, email));
        }


        public AccountEmailTools(IEmailSender emailSender, ILoggerFactory loggerFactory)
        {
            if (emailSender == null || loggerFactory == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            _emailSender = emailSender;
            _logger = loggerFactory.CreateLogger<AccountEmailTools>();
        }
    }
}
