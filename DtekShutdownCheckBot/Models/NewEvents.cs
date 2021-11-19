using System.Collections.Generic;
using DtekShutdownCheckBot.Models.Entities;
using MediatR;

namespace DtekShutdownCheckBot.Models
{
	public class NewEvents : INotification
	{
        public NewEvents()
        {

        }


        public NewEvents(IList<long> chatIds,  IList<Shutdown> shutdowns)
        {
	        ChatIds = chatIds;
            Shutdowns = shutdowns;
        }

        public IList<long> ChatIds { get; }

        public IList<Shutdown> Shutdowns { get; }
	}
}
