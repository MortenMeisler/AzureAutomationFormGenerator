using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AzureAutomationFormGenerator.WebUI.Repos
{
    public class MessageSender : Hub, IMessageSender
    {
        //todo: might be shared/not thread safe, are we scoped pr user? though messagesender is added pr. scope aka websession as DI
        private static string connectionId;

        private readonly IHubContext<MessageSender> _signalHubContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //lotto
        public override Task OnConnectedAsync()
        {
            
            _signalHubContext.Groups.AddToGroupAsync(Context.ConnectionId, _httpContextAccessor.HttpContext.Request.Cookies[".AspNetCore.Antiforgery.10K41XpMUM8"]);
           
            return base.OnConnectedAsync();
        }

        public string GetConnectionId()
        {
           
            return Context.ConnectionId;
        }

        public MessageSender(IHubContext<MessageSender> signalHubContext, IHttpContextAccessor httpContextAccessor)
        {
            _signalHubContext = signalHubContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task SendErrorMessage(string message)
        {
            await _signalHubContext.Clients.Group(_httpContextAccessor.HttpContext.Request.Cookies[".AspNetCore.Antiforgery.10K41XpMUM8"]).SendAsync("initMessage", message, connectionId);
        }

        public async Task SendMessage(string message)
        { 
            await _signalHubContext.Clients.Group(_httpContextAccessor.HttpContext.Request.Cookies[".AspNetCore.Antiforgery.10K41XpMUM8"]).SendAsync("initMessage", message, connectionId);
        }

        public async Task SendStatus(string status)
        {
            await _signalHubContext.Clients.Group(_httpContextAccessor.HttpContext.Request.Cookies[".AspNetCore.Antiforgery.10K41XpMUM8"]).SendAsync("initMessage", status);
        }
    }
}
