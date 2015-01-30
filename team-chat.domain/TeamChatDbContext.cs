using System.Data.Entity;
using team_chat.domain.models;

namespace team_chat.domain
{
    public class TeamChatDbContext:DbContext
    {
        public DbSet<ChatMessage> ChatMessages { get; set; } 
    }
}