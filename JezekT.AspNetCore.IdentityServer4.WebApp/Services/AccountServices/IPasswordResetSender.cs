﻿using System.Threading.Tasks;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Services.AccountServices
{
    public interface IPasswordResetSender
    {
        Task SendPasswordResetAsync(string email, string callbackUrl);
    }
}
