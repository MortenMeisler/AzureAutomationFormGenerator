using AzureAutomationFormGenerator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureAutomationFormGenerator.Repos
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
            await SendMessage(Configuration["AzureSettings:OutputMessageJobStarted"]);
        }

        //Determines the type of view to be returned - ex. full width (default) or centered
        public enum PageType
        {

            FullWidth,
            Centered
        }
        public static PageType currentPageType { get; set; }


    }

    
}
