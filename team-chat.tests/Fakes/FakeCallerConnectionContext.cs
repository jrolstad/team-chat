using System.Dynamic;
using Microsoft.AspNet.SignalR.Hubs;

namespace team_chat.tests.Fakes
{
    public class FakeCallerConnectionContext<T> : IHubCallerConnectionContext<T>
    {
        private dynamic _caller = new ExpandoObject();
        private dynamic _allClients = new ExpandoObject();

        public FakeCallerConnectionContext<T> WithCaller(T caller)
        {
            _caller = caller;

            return this;
        }

        public FakeCallerConnectionContext<T> WithAllClients(T client)
        {
            _allClients = client;

            return this;
        }

        public T Caller
        {
            get { return _caller; }
        }

        public dynamic CallerState
        {
            get { throw new System.NotImplementedException(); }
        }

        public T Others
        {
            get { throw new System.NotImplementedException(); }
        }

        public T OthersInGroup(string groupName)
        {
            throw new System.NotImplementedException();
        }

        public T OthersInGroups(System.Collections.Generic.IList<string> groupNames)
        {
            throw new System.NotImplementedException();
        }

        public T All
        {
            get { return _allClients; }
        }

        public T AllExcept(params string[] excludeConnectionIds)
        {
            return _allClients;
        }

        public T Client(string connectionId)
        {
            throw new System.NotImplementedException();
        }

        public T Clients(System.Collections.Generic.IList<string> connectionIds)
        {
            throw new System.NotImplementedException();
        }

        public T Group(string groupName, params string[] excludeConnectionIds)
        {
            throw new System.NotImplementedException();
        }

        public T Groups(System.Collections.Generic.IList<string> groupNames, params string[] excludeConnectionIds)
        {
            throw new System.NotImplementedException();
        }

        public T User(string userId)
        {
            throw new System.NotImplementedException();
        }

        public T Users(System.Collections.Generic.IList<string> userIds)
        {
            throw new System.NotImplementedException();
        }
    }
}