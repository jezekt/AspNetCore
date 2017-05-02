using System.ComponentModel.DataAnnotations;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username", ResourceType = typeof(Resources.Models.AccountViewModels.LoginViewModel))]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Resources.Models.AccountViewModels.LoginViewModel))]
        public string Password { get; set; }
        [Display(Name = "RememberLogin", ResourceType = typeof(Resources.Models.AccountViewModels.LoginViewModel))]
        public bool RememberLogin { get; set; }
        public string ReturnUrl { get; set; }
    }
}