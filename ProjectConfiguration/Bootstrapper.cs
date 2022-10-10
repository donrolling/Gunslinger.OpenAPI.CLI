using Domain.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectConfiguration.Extensions;
using Serilog;
using System.Net.Http.Headers;

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
				})
				.UseSerilog((hostContext, services, loggerConfiguration) =>
				{
					var appSettings = hostContext.Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();
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