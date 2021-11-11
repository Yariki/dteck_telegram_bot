namespace DtekShutdownCheckBot.Services
{
	public interface IServiceFactory
	{
		T Get<T>();
	}
}
