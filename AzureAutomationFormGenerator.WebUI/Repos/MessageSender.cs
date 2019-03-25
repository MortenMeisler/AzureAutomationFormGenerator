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
        
        public string ConnectionId { get; set; }

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHubContext<MessageSender> _signalHubContext;

        public MessageSender(IHttpContextAccessor httpContextAccessor, IHubContext<MessageSender> signalHubContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _signalHubContext = signalHubContext;
        }

        public override Task OnConnectedAsync()
        {

            //Groups.AddToGroupAsync(Context.ConnectionId, _httpContextAccessor.HttpContext.Request.Cookies["SomeCookieCreatedForSessionX"]);

            return base.OnConnectedAsync();
        }
        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }

        public async Task SendErrorMessage(string message)
        {
            await _signalHubContext.Clients.Client(ConnectionId).SendAsync("initErrorMessage", message, ConnectionId);
        }
    
    
        public async Task SendMessage(string message)
        {

            await _signalHubContext.Clients.Client(ConnectionId).SendAsync("initMessage", message, ConnectionId);
            //await Clients.Group(_httpContextAccessor.HttpContext.Request.Cookies[".AspNetCore.Antiforgery.10K41XpMUM8"]).SendAsync("initMessage", message, connectionId);
        }

        public async Task SendStatus(string status)
        {
            await _signalHubContext.Clients.Client(ConnectionId).SendAsync("initStatus", status);
            //await Clients.Group(_httpContextAccessor.HttpContext.Request.Cookies[".AspNetCore.Antiforgery.10K41XpMUM8"]).SendAsync("initMessage", status);
        }

    }
}
