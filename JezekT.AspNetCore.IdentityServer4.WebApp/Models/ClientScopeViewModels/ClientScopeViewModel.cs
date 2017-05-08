﻿using System.ComponentModel.DataAnnotations;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientViewModels;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientScopeViewModels
{
    public class ClientScopeViewModel : ClientStringSettingsViewModel
    {
        [Display(Name = "Scope", ResourceType = typeof(Resources.Models.ClientScopeViewModels.ClientScopeViewModel))]
        public string Scope { get; set; }
        public override string Value => Scope;
    }
}
