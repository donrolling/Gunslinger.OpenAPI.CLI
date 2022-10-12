using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OutputTests.BaseClasses
{
	public class IntegrationTestBase
	{
		protected IHost Host { get; private set; }

		protected T GetService<T>()
		{
			return Host.Services.GetService<T>();
		}

		public IntegrationTestBase()
		{
			Host = Bootstrapper.GetHost(new string[] { }).Host;
		}
	}
}