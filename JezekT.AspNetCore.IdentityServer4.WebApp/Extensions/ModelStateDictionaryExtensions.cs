using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Extensions
{
    public static class ModelStateDictionaryExtensions
    {
        public static void AddErrors(this ModelStateDictionary modelState, IList<IdentityError> errors)
        {
            if (errors != null)
            {
                foreach (var err in errors)
                {
                    modelState.AddModelError(err.Code, err.Description);
                }
            }
        }
    }
}
