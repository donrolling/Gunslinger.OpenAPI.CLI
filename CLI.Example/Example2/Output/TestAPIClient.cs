using Domain;
using Microsoft.Extensions.Logging;
using OutputTests.Client.Models;
using System.Text;
using System.Text.Json;

namespace OutputTests.Client 
{
    public class TestAPIClient : ITestAPIClient
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;
        private readonly ILogger<TestAPIClient> _logger;

		private static readonly JsonSerializerOptions _options;

        static TestAPIClient()
        { 
            _options = new JsonSerializerOptions
		    {
			    PropertyNameCaseInsensitive = true
		    };
        }

        public TestAPIClient(string baseUrl, HttpClient httpClient, ILogger<TestAPIClient> logger)
        {
            _baseUrl = baseUrl;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<OperationResult> PostOriginationGetChaseYetToFundEmailCandidatesAndEmailAsync(int lomHoursOldbool affiliate)
        {
            var urlBuilder = GetUrl("/Origination/GetChaseYetToFundEmailCandidatesAndEmail");
            urlBuilder.Append("?");
            urlBuilder.Append("lomHoursOld=");
            urlBuilder.Append(lomHoursOld.ToString());
            urlBuilder.Append("&");
            urlBuilder.Append("affiliate=");
            urlBuilder.Append(affiliate.ToString());
            urlBuilder.Append("&");
            urlBuilder.Length--;
            var url = urlBuilder.ToString().ToLower();
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), new Uri(url, UriKind.RelativeOrAbsolute)))
			{
				var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                var status = (int)response.StatusCode;
                if (response.IsSuccessStatusCode)
				{
					return OperationResult.Ok();
				}
                else 
                {
                    return OperationResult.Fail($"Error - Status {status}");
                }
			}
        }
        
        private StringBuilder GetUrl(string path) 
        {
            var urlBuilder = new StringBuilder();
            urlBuilder.Append(_baseUrl != null ? _baseUrl.TrimEnd('/') : "").Append(path);
            return urlBuilder;
        }
    }
}