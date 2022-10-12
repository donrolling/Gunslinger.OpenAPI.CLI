using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OutputTests.Client.Models;

namespace OutputTests.Client.Configuration 
{    
	public static class ConfigurationHelper
	{
		private const string APIClientName = "TestAPIClient";

		public static void ConfigureTestAPIClient(this IServiceCollection services, string baseUrl)
		{
			services.AddHttpClient(APIClientName, httpClient =>
			{
				httpClient.DefaultRequestHeaders.Accept.Clear();
				httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			});
			services.AddTransient<ITestAPIClient>(a =>
			{
				var httpClientFactory = a.GetService<IHttpClientFactory>();
				var httpClient = httpClientFactory.CreateClient(APIClientName);
				var logger = a.GetService<ILogger<TestAPIClient>>();
				return new TestAPIClient(baseUrl, httpClient, logger);
			});
		}
	}
}