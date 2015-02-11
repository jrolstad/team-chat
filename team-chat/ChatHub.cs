using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using team_chat.domain;
using team_chat.domain.models;

namespace team_chat
{
    public class ChatHub : Hub
    {
        private readonly TeamChatDbContext _dbContext;

        public ChatHub():this(new TeamChatDbContext())
        {
            
        }

        public ChatHub(TeamChatDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Send(string message)
        {
            if(string.IsNullOrWhiteSpace(message))
                return;

            var chatMessage = CreateNewMessage(message);

            dynamic callingClient = Clients.Caller;
            BroadcastMessageToClient(callingClient,chatMessage,ShowNotification.No);

            dynamic allClientsExceptCaller = Clients.AllExcept(Context.ConnectionId);
            BroadcastMessageToClient(allClientsExceptCaller, chatMessage, ShowNotification.Yes);
            
            _dbContext.ChatMessages.Add(chatMessage);
            _dbContext.SaveChanges();
        }

        private ChatMessage CreateNewMessage(string message)
        {
            var userName = GetUserName();
            var chatMessage = new ChatMessage {Sender = userName, Message = message, SentAt = DateTime.UtcNow};
            return chatMessage;
        }

        public override Task OnConnected()
        {
            ShowAllMessagesOnCaller();

            return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            ResetMessagesOnClient(this.Clients.Caller);
            ShowAllMessagesOnCaller();

            return base.OnReconnected();
        }

        private void ShowAllMessagesOnCaller()
        {
            var messages = _dbContext.ChatMessages;
            BroadcastMessagesToClient(this.Clients.Caller, messages);
        }

        private void ResetMessagesOnClient(dynamic client)
        {
            client.resetMessages();
        }

        private void BroadcastMessageToClient(dynamic client, ChatMessage message, bool showNotification)
        {
            client.broadcastMessage(message, showNotification);
        }

        private void BroadcastMessagesToClient(dynamic client, IEnumerable<ChatMessage> messages)
        {
            var messageArray = messages.ToArray();
            client.broadcastMessages(messageArray);
        }

        private string GetUserName()
        {
            var userName = "N/A";

            if (this.Context != null
                && this.Context.User != null
                && this.Context.User.Identity != null)
            {
                userName = this.Context.User.Identity.Name;
            }
            return userName;
        }

        private static class ShowNotification
        {
            public const bool Yes = true;
            public const bool No = false;
        }
    }
}