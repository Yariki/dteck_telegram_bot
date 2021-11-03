using System;
using LiteDB;

namespace DtekShutdownCheckBot.Models.Entities
{
    public class Shutdown
    {
        [BsonId]
        public string Id { get; set; }

        public DateTime ShutdownDate { get; set; }

        public string City { get; set; }

    }
}