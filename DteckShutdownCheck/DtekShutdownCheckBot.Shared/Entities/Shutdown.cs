using System;
using LiteDB;

namespace DtekShutdownCheckBot.Shared.Entities
{
    public class Shutdown
    {
        public Shutdown(DateTime shutdownDate, string city, string timeOfTheEvent)
        {
            ShutdownDate = shutdownDate;
            City = city;
            TimeOfTheEvent = timeOfTheEvent;
            Id = Guid.NewGuid().ToString();
            Hashcode = City.GetHashCode() ^ ShutdownDate.GetHashCode();
            IsSent = false;
        }
        
        [BsonId]
        public string Id { get; set; }

        public DateTime ShutdownDate { get; set; }

        public string City { get; set; }

        public long Hashcode { get; set; }

        public bool IsSent { get; set; }

        public string TimeOfTheEvent { get; set; }

    }
}
