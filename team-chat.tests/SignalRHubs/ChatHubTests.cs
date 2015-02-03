using System;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using NUnit.Framework;
using team_chat.domain;
using team_chat.domain.models;
using team_chat.tests.Fakes;

namespace team_chat.tests.SignalRHubs
{
    [TestFixture]
    public class ChatHubTests
    {
        [Test]
        public void Send_SendsMessageToAllUsers()
        {
            // Arrange
            var dbContext = GetDbContext();
            var hub = new ChatHub(dbContext);

            var clients = new FakeCallerConnectionContext<dynamic>();
            dynamic callingClient = GetFakeClient();
            dynamic allClients = GetFakeClient();

            clients.WithCaller(callingClient);
            clients.WithAllClients(allClients);

            hub.Clients = clients;

            var context = new FakeHubCallerContext();
            context.WithUser(new GenericPrincipal(new GenericIdentity("My Name"),null));
           
            hub.Context = context;
            
            // Act
            hub.Send("Some message");

            // Assert
            Assert.That(callingClient.sentMessage,Is.Not.Null,"Message not sent to calling client");
            Assert.That(callingClient.sentMessage.Message, Is.EqualTo("Some message"), "Message text not sent correctly to calling client");
            Assert.That(callingClient.notificationShown, Is.False, "The calling client shouldn't be notified of their own message");

            Assert.That(allClients.sentMessage, Is.Not.Null, "Message not sent to other clients");
            Assert.That(allClients.sentMessage.Message, Is.EqualTo("Some message"), "Message text not sent correctly to other clients");
            Assert.That(allClients.notificationShown, Is.True, "The other clients should be notified of the message");

            Assert.That(dbContext.ChatMessages.Count(),Is.EqualTo(1),"Message not saved to the database");
            Assert.That(dbContext.ChatMessages.First().Message,Is.EqualTo("Some message"), "Message text not saved correctly");
            Assert.That(dbContext.ChatMessages.First().Sender,Is.EqualTo("My Name"), "Sender name not set correctly");

        }

        [Test]
        public void OnConnected_ShowsAllPastMessagesOnCaller()
        {
            // Arrange
            var dbContext = GetDbContext();
            dbContext.ChatMessages.Add(new ChatMessage {Message = "one",Sender = "uno",SentAt = DateTime.Now.AddDays(-1)});
            dbContext.ChatMessages.Add(new ChatMessage {Message = "two",Sender = "uno",SentAt = DateTime.Now.AddDays(-2)});
            dbContext.SaveChanges();

            var hub = new ChatHub(dbContext);

            var clients = new FakeCallerConnectionContext<dynamic>();
            dynamic callingClient = GetFakeClient();
            dynamic allClients = GetFakeClient();

            clients.WithCaller(callingClient);
            clients.WithAllClients(allClients);

            hub.Clients = clients;

            // Act
            hub.OnConnected();

            // Assert
            Assert.That(callingClient.messagesReset, Is.False, "Messages should not be reset when connecting");
            Assert.That(callingClient.sentMessageCount, Is.EqualTo(2), "All messages should be sent");

            Assert.That(allClients.sentMessageCount, Is.EqualTo(0), "Only the calling client should have messages sent");
            Assert.That(allClients.messagesReset, Is.False, "Messages should not be reset on all callers when connecting");
        }

        [Test]
        public void OnReconnected_ShowsAllPastMessagesOnCaller()
        {
            // Arrange
            var dbContext = GetDbContext();
            dbContext.ChatMessages.Add(new ChatMessage { Message = "one", Sender = "uno", SentAt = DateTime.Now.AddDays(-1) });
            dbContext.ChatMessages.Add(new ChatMessage { Message = "two", Sender = "uno", SentAt = DateTime.Now.AddDays(-2) });
            dbContext.SaveChanges();

            var hub = new ChatHub(dbContext);

            var clients = new FakeCallerConnectionContext<dynamic>();
            dynamic callingClient = GetFakeClient();
            dynamic allClients = GetFakeClient();

            clients.WithCaller(callingClient);
            clients.WithAllClients(allClients);

            hub.Clients = clients;

            // Act
            hub.OnReconnected();

            // Assert
            Assert.That(callingClient.messagesReset, Is.True, "Messages should be reset when connecting so we can resend everything");
            Assert.That(callingClient.sentMessageCount, Is.EqualTo(2), "All messages should be sent");

            Assert.That(allClients.sentMessageCount, Is.EqualTo(0), "Only the calling client should have messages sent");
            Assert.That(allClients.messagesReset, Is.False, "Messages should not be reset on all callers when connecting");
        }

        private TeamChatDbContext GetDbContext()
        {
            return new TeamChatDbContext(Effort.DbConnectionFactory.CreateTransient());
        }

        private dynamic GetFakeClient()
        {
            dynamic client = new ExpandoObject();
            client.sentMessageCount = 0;
            client.messagesReset = false;

            client.broadcastMessage = new Action<ChatMessage, bool>((message, showNotification) =>
            {
                client.sentMessage = message;
                client.notificationShown = showNotification;
                client.sentMessageCount++;
            });

            client.resetMessages = new Action (() =>
            {
                client.messagesReset = true;
            });

            return client;
        }
    }
}