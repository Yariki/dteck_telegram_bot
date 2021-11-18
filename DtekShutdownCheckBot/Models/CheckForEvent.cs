using System.Collections.Generic;
using MediatR;

namespace DtekShutdownCheckBot.Models
{
	public class CheckForEvent : INotification
	{
		public CheckForEvent(IList<long> chatIds)
		{
			ChatIds = chatIds;
		}

		public IList<long> ChatIds { get; }
    }
}
