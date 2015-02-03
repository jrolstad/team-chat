using System.Data.Common;
using System.Data.Entity;
using team_chat.domain.models;

namespace team_chat.domain
{
    public class TeamChatDbContext:DbContext
    {
        public TeamChatDbContext():base()
        {
            
        }

        public TeamChatDbContext(DbConnection createTransient):base(createTransient,true)
        {
           
        }

        public DbSet<ChatMessage> ChatMessages { get; set; } 
    }
}