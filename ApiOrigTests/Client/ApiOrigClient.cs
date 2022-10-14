using Domain;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace ApiOrigTests.Client 
{
    public class ApiOrigClient : IApiOrigClient
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiOrigClient> _logger;

		private static readonly JsonSerializerOptions _options;

        static ApiOrigClient()
        { 
            _options = new JsonSerializerOptions
		    {
			    PropertyNameCaseInsensitive = true
		    };
        }

        public ApiOrigClient(string baseUrl, HttpClient httpClient, ILogger<ApiOrigClient> logger)
        {
            _baseUrl = baseUrl;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<OperationResult> PostOriginationGetChaseYetToFundEmailCandidatesAndEmailAsync(int lomHoursOld, bool affiliate)
        {
            var urlBuilder = GetUrl("/Origination/GetChaseYetToFundEmailCandidatesAndEmail");
            urlBuilder.Append("?");
            urlBuilder.Append("LomHoursOld=");
            urlBuilder.Append(lomHoursOld.ToString());
            urlBuilder.Append("&");
            urlBuilder.Append("Affiliate=");
            urlBuilder.Append(affiliate.ToString());
            
            var url = urlBuilder.ToString();
            try
            {
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
            catch (Exception ex)
            {
                var message = $"Error - Post.PostOriginationGetChaseYetToFundEmailCandidatesAndEmailAsync - {ex.Message}";
                _logger.LogError(message);
                return OperationResult.Fail(message);
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