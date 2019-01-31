using AzureAutomationFormGenerator.WebUI.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureAutomationFormGenerator.WebUI.Security
{

    public static class AzureADPolicies
    {
        public static string Name => "AzureADAuthorizationRequired";

        public static void Build(AuthorizationPolicyBuilder builder)
            {
                var section = StaticRepo.Configuration.GetSection($"AzureAd:AuthorizedAdGroups");
                var groups = section.Get<string[]>();
                builder.RequireClaim("groups", groups); 
            }
    }
}
