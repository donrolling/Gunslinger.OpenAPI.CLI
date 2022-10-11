using Enterprise.Configuration.Extensions;
using IntegrationTestProject.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace IntegrationTestProject
{
	public static class Bootstrapper
	{
		private const string serviceName = $"TODO: Put project name here";

		public static (IHost Host, string ServiceName) GetHost(string[] args)
		{
			var hostBuilder = Host.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration((context, builder) =>
				{
					var path = "appsettings.json";
					builder.AddJsonFile(path, optional: false, reloadOnChange: true);
				})
				.ConfigureServices((hostContext, services) =>
				{
					services.AddConfiguration<AppSettings>(hostContext, nameof(AppSettings));
				})
				.UseSerilog((hostContext, services, loggerConfiguration) =>
				{
					var appSettings = hostContext.Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();
					var logPath = Path.Combine(AppContext.BaseDirectory, appSettings.LogFilePath).Replace("\\", "/");
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