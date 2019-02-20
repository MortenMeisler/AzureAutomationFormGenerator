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


        public static IHubContext<SignalHub> HubContext { get; set; }

        public static IConfiguration Configuration { get; set; }

        public async static Task SendErrorMessage(string message)
        {
            await HubContext.Clients.Client(ConnectionId).SendAsync("initErrorMessage", message);
        }

        public async static Task SendMessage(string message)
        {
            await HubContext.Clients.Client(ConnectionId).SendAsync("initMessage", message);
          
        }

        public async static Task SendMessageJobStartedSuccessfully()
        {
            await SendMessage(Configuration["Text:OutputMessageJobStarted"]);
        }

        //Determines the type of view to be returned - ex. full width (default) or centered
        public enum PageType
        {

            Default,
            FullWidth,
            Centered
        }
        public static PageType currentPageType { get; set; }

        
        public static IList<RunbookSimple> runbooks { get; set; }

    }

    
}
