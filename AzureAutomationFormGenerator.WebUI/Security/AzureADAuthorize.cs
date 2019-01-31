using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureAutomationFormGenerator.WebUI.Security
{
    public class AzureADAuthorize : AuthorizeAttribute
    {
        public AzureADAuthorize() : base(AzureADPolicies.Name)
        {
        }

    }
}
