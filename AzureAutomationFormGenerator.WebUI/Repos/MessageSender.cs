using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureAutomationFormGenerator.WebUI.Repos
{
    public class MessageSender : Hub, IMessageSender
    {
        //todo: might be shared/not thread safe, are we scoped pr user? though messagesender is added pr. scope aka websession as DI
        private static string connectionId;
        private readonly IHubContext<MessageSender> _signalHubContext;
        public override Task OnConnectedAsync()
        {
            connectionId = Context.ConnectionId;
            return base.OnConnectedAsync();
        }

        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }

        public MessageSender(IHubContext<MessageSender> signalHubContext)
        {
            _signalHubContext = signalHubContext;
        }

        public async Task SendErrorMessage(string message)
        {
            await _signalHubContext.Clients.Client(connectionId).SendAsync("initErrorMessage", message);
        }

        public async Task SendMessage(string message)
        {
            await _signalHubContext.Clients.Client(connectionId).SendAsync("initMessage", message, connectionId);
        }

        public async Task SendStatus(string status)
        {
            await _signalHubContext.Clients.Client(connectionId).SendAsync("initStatus", status);
        }
    }
}
