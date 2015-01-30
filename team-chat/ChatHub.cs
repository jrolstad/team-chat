using System;
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
            var userName = GetUserName();
            var chatMessage = new ChatMessage {Sender = userName, Message = message, SentAt = DateTime.UtcNow};
            Clients.All.broadcastMessage(chatMessage);

            _dbContext.ChatMessages.Add(chatMessage);
            _dbContext.SaveChanges();
        }

        public override Task OnConnected()
        {
            foreach (var message in _dbContext.ChatMessages)
            {
                this.Clients.Caller.broadcastMessage(message);
            }

            return base.OnConnected();
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