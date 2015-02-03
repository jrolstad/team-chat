using System.Security.Principal;
using Microsoft.AspNet.SignalR.Hubs;

namespace team_chat.tests.Fakes
{
    public class FakeHubCallerContext:HubCallerContext
    {
        private string _connectionId;
        private IPrincipal _user;

        public override string ConnectionId
        {
            get
            {
                return _connectionId;
            }
        }

        public FakeHubCallerContext WithConnectionId(string connectionId)
        {
            _connectionId = connectionId;

            return this;
        }

        public override IPrincipal User
        {
            get { return _user; }
        }

        public FakeHubCallerContext WithUser(IPrincipal user)
        {
            _user = user;

            return this;
        }
    }
}