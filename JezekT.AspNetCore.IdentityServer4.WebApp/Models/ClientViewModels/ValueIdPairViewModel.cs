using System.ComponentModel.DataAnnotations;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientViewModels
{
    public class ValueIdPairViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Value", ResourceType = typeof(Resources.Models.ClientViewModels.ValueIdPairViewModel))]
        public string Value { get; set; }
    }
}
