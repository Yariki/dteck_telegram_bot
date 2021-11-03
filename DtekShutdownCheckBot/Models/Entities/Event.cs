using LiteDB;

namespace DtekShutdownCheckBot.Models.Entities
{
    public class Event
    {
        [BsonId]
        public string Id { get; set; }

        public long ChatId { get; set; }

        public string EventsId { get; set; }
        
    }
}