﻿using System;
using System.Diagnostics.Contracts;
using JezekT.NetStandard.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace JezekT.AspNetCore.Mvc.Extensions
{
    public static class ServiceErrorsProviderExtensions
    {
        public static void ResolveErrors(this IServiceErrorsProvider serviceErrorsProvider, ModelStateDictionary modelState)
        {
            if (modelState == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            if (serviceErrorsProvider.HasValidationError)
            {
                var validationErrors = serviceErrorsProvider.GetValidationErrors();
                foreach (var error in validationErrors)
                {
                    modelState.AddModelError(error.Key, error.Value);
                }
            }
            if (!string.IsNullOrEmpty(serviceErrorsProvider.ExceptionMessage))
            {
                modelState.AddModelError("Exception", serviceErrorsProvider.ExceptionMessage);
            }
        }
    }
}
