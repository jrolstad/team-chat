using System;
using System.ComponentModel.DataAnnotations;

namespace team_chat.domain.models
{
    public class ChatMessage
    {
        [Key]
        public long MessageId { get; set; }

        public string Sender { get; set; }

        public string Message { get; set; }

        public DateTime SentAt { get; set; }
    }
}