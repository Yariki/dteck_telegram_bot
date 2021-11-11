using System;

namespace DtekShutdownCheckBot.Services
{
	public class ServiceFactory : IServiceFactory
	{

		private Func<Type, object> _factory;


		public ServiceFactory(Func<Type, object> factory)
		{
			_factory = factory;
		}

		public T Get<T>()
		{
			return (T)_factory(typeof(T));
		}
	}
}
