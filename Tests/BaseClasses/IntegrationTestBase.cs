using Domain;
using Domain.Configuration;
using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjectConfiguration;
using Tests.Extensions;

namespace Tests.BaseClasses
{
	public abstract class IntegrationTestBase
	{
		public TestContext TestContext { get; set; }

		protected IHost Host { get; private set; }
		protected string RootPath { get; private set; }

		public IntegrationTestBase()
		{
			RootPath = Environment.CurrentDirectory;
			Host = Bootstrapper.GetHostConfiguration().Host;
		}

		protected CommandSettings GetCommandSettings(TestContext testContext, string filename)
		{
			var path = testContext.GetTestInputFullPath(filename);
			return new CommandSettings
			{
				ConfigPath = path,
				RootPath = RootPath
			};
		}

		protected T GetService<T>()
		{
			RootPath = Environment.CurrentDirectory;
			return Host.Services.GetService<T>();
		}

		protected async Task<OperationResult> RunGeneratorAsync<T>(T testClass, TestContext testContext, string filename)
		{
			using (var scope = Host.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				var logger = services.GetRequiredService<ILogger<T>>();
				var generatorService = services.GetRequiredService<IGenerationManager>();
				var commandSettings = GetCommandSettings(testContext, filename);
				return await generatorService.GenerateAsync(commandSettings);
			}
		}
	}
}