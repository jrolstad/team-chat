﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
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

            var userName = GetUserName();
            var chatMessage = new ChatMessage {Sender = userName, Message = message, SentAt = DateTime.UtcNow};

            Clients.Caller.broadcastMessage(chatMessage, false);
            Clients.AllExcept(Context.ConnectionId).broadcastMessage(chatMessage, true);

            _dbContext.ChatMessages.Add(chatMessage);
            _dbContext.SaveChanges();
        }

        public override Task OnConnected()
        {
            BroadcastAllMessages();

            return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            this.Clients.Caller.resetMessages();
            BroadcastAllMessages();


            return base.OnReconnected();
        }

        private void BroadcastAllMessages()
        {
            foreach (var message in _dbContext.ChatMessages)
            {
                this.Clients.Caller.broadcastMessage(message,false);
            }
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
    }
}