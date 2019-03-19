using AzureAutomationFormGenerator.WebUI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Management.Automation.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.Rest.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureAutomationFormGenerator.WebUI.Repos
{
    public static class StaticRepo
    {
        public static string RunbookName { get; set; }

        public static string AutomationAccount { get; set; }

        public static string ResourceGroup { get; set; }

        public static string ConnectionId { get; set; }

        public static IConfiguration Configuration { get; set; }
        
        //Determines the type of view to be returned - ex. full width (default) or centered
        public enum PageType
        {
            Default,
            FullWidth,
            Centered
        }
        public static PageType CurrentPageType { get; set; }


    }

    
}
