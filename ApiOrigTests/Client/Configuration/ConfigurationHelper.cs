using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace ApiOrigTests.Client.Configuration 
{    
	public static class ConfigurationHelper
	{
		private const string APIClientName = "ApiOrigClient";

		public static void ConfigureApiOrigClient(this IServiceCollection services, string baseUrl)
		{
			services.AddHttpClient(APIClientName, httpClient =>
			{
				httpClient.DefaultRequestHeaders.Accept.Clear();
				httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			});
			services.AddTransient<IApiOrigClient>(a =>
			{
				var httpClientFactory = a.GetService<IHttpClientFactory>();
				var httpClient = httpClientFactory.CreateClient(APIClientName);
				var logger = a.GetService<ILogger<ApiOrigClient>>();
				return new ApiOrigClient(baseUrl, httpClient, logger);
			});
		}
	}
}