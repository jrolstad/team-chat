using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace team_chat
{
    public class ChatHub : Hub
    {
        public void Send(string message)
        {
            var userName = GetUserName();
            Clients.All.broadcastMessage(userName, message);

            _dbContext.Messages.Add(message);
            saveChanges
        }

        public override Task OnConnected()
        {
            var messages = _dbContext.Messages();

            this.Clients.Caller.broadcastMessage("foo", "connection message");

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