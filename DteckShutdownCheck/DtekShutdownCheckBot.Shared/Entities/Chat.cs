using System.Collections.Generic;
using LiteDB;

namespace DtekShutdownCheckBot.Shared.Entities
{
    public class Chat
    {
        [BsonId]
        public string Id { get; set; }

        public long ChatId { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        
        public string[] Words { get; set; }

    }
}