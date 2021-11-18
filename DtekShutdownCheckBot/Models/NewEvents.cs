using DtekShutdownCheckBot.Models.Entities;
using MediatR;

namespace DtekShutdownCheckBot.Models
{
	public class NewEvents : INotification
	{
        public NewEvents()
        {
            
        }

        public NewEvents(List<Shutdown> shutdowns)
        {
            Shutdowns = shutdowns;
        }

        public List<Shutdown> Shutdowns { get; }
	}
}
