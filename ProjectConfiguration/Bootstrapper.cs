using Business.Engines;
using Business.Factories;
using Business.Managers;
using Business.Providers;
using Domain.Configuration;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectConfiguration.Extensions;
using Serilog;

namespace ProjectConfiguration
{
	public class Bootstrapper
	{
		public static (IHost Host, string ServiceName) GetHostConfiguration(string baseDirectory = "")
		{
			var serviceName = "Gunslinger.OpenAPI.CLI";
			var hostBuilder = Host.CreateDefaultBuilder()
				.ConfigureAppConfiguration((context, builder) =>
				{
					var path = $"{baseDirectory}appsettings.json";
					builder.AddJsonFile(path, optional: false, reloadOnChange: true);
				})
				.ConfigureServices((hostContext, services) =>
				{
					services.AddConfiguration<AppSettings>(hostContext, nameof(AppSettings));
					services.AddHttpClient();

					// add services
					services.AddTransient<IGenerationManager, GenerationManager>();
					services.AddTransient<IContextFactory, ContextFactory>();
					services.AddTransient<IFileTemplateProvider, FileTemplateProvider>();
					services.AddTransient<IOpenApiParsingEngine, OpenApiParsingEngine>();
					services.AddTransient<IRenderEngine, HandlebarsRenderEngine>();
					services.AddTransient<IFileCreationEngine, FileCreationEngine>();
				})
				.UseSerilog((hostContext, loggerConfiguration) =>
				{
					var logPath = Path.Combine(AppContext.BaseDirectory, "Logs/log.txt").Replace("\\", "/");
					loggerConfiguration
						.ReadFrom.Configuration(hostContext.Configuration)
						.Enrich.WithProperty("Service Name", serviceName)
						.WriteTo.File(logPath, rollingInterval: RollingInterval.Day);
				});
			var host = hostBuilder.Build();
			return (host, serviceName);
		}
	}
}