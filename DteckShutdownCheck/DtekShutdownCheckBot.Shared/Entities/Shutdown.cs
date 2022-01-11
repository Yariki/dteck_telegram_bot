using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LiteDB;

namespace DtekShutdownCheckBot.Shared.Entities
{
    public class Shutdown
    {
        public Shutdown(DateTime shutdownDate, string city, string timeOfTheEvent, string streets)
        {
            ShutdownDate = shutdownDate;
            City = city;
            Streets = streets;
            TimeOfTheEvent = timeOfTheEvent;
            Hashcode = City.GetHashCode() ^ ShutdownDate.GetHashCode();
            IsSent = false;
        }
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime ShutdownDate { get; set; }

        public string City { get; set; }

        public string Streets { get; set; }

        public long Hashcode { get; set; }

        public bool IsSent { get; set; }

        public string TimeOfTheEvent { get; set; }

    }
}
