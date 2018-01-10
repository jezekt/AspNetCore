using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Data
{
    public class User : IdentityUser
    {
        public virtual ICollection<IdentityUserRole<string>> Roles { get; } = new List<IdentityUserRole<string>>();
        public virtual ICollection<IdentityUserClaim<string>> Claims { get; } = new List<IdentityUserClaim<string>>();
        public virtual ICollection<IdentityUserLogin<string>> Logins { get; } = new List<IdentityUserLogin<string>>();
    }
}
