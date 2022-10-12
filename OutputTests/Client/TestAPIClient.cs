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

        public TestAPIClient(string baseUrl, HttpClient httpClient, ILogger<TestAPIClient> logger)
        {
            _baseUrl = baseUrl;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<OperationResult<Course>> GetCourseAsync(int id)
        {
            var urlBuilder = GetUrl("/api/Course");
            urlBuilder.Append("?");
            urlBuilder.Append(id.ToString());
            urlBuilder.Append("&");
            urlBuilder.Length--;
            var url = urlBuilder.ToString();
            using (var request = new HttpRequestMessage(new HttpMethod("GET"), new Uri(url, UriKind.RelativeOrAbsolute)))
			{
				var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                var status = (int)response.StatusCode;
                if (response.IsSuccessStatusCode)
				{
                    try {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonSerializer.Deserialize<Course>(content);
                        return OperationResult.Ok(result);
                    }
                    catch(Exception ex)
                    {
                        var message = $"Error in response. Status {status}. Error: {ex.Message}";
                        _logger.LogError(message);
                        return OperationResult.Fail<Course>(message);
                    }
				}
                else 
                {
                    return OperationResult.Fail<Course>($"Error - Status {status}");
                }
			}
        }
        
        public async Task<OperationResult<Course>> PostCourseAsync(Course course)
        {
            var url = GetUrl("/api/Course").ToString();
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), new Uri(url, UriKind.RelativeOrAbsolute)))
			{
                var body = JsonSerializer.Serialize(course);
				request.Content = new StringContent(body, Encoding.UTF8, "application/json");
				var response = await _httpClient.SendAsync(request);
                var status = (int)response.StatusCode;
                if (response.IsSuccessStatusCode)
				{
                    try {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonSerializer.Deserialize<Course>(content);
					    return OperationResult.Ok(result);
                    }
                    catch(Exception ex)
                    {
                        var message = $"Error in response. Status {status}. Error: {ex.Message}";
                        _logger.LogError(message);
                        return OperationResult.Fail<Course>(message);
                    }
				}
                else 
                {
                    return OperationResult.Fail<Course>($"Error - Status {status}");
                }
			}
        }
        
        public async Task<OperationResult<Course>> PutCourseAsync(Course course)
        {
            var url = GetUrl("/api/Course").ToString();
            using (var request = new HttpRequestMessage(new HttpMethod("PUT"), new Uri(url, UriKind.RelativeOrAbsolute)))
			{
                var body = JsonSerializer.Serialize(course);
				request.Content = new StringContent(body, Encoding.UTF8, "application/json");
				var response = await _httpClient.SendAsync(request);
                var status = (int)response.StatusCode;
                if (response.IsSuccessStatusCode)
				{
                    try {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonSerializer.Deserialize<Course>(content);
					    return OperationResult.Ok(result);
                    }
                    catch(Exception ex)
                    {
                        var message = $"Error in response. Status {status}. Error: {ex.Message}";
                        _logger.LogError(message);
                        return OperationResult.Fail<Course>(message);
                    }
				}
                else 
                {
                    return OperationResult.Fail<Course>($"Error - Status {status}");
                }
			}
        }
        
        public async Task<OperationResult> DeleteCourseAsync(int id)
        {
            var urlBuilder = GetUrl("/api/Course");
            urlBuilder.Append("?");
            urlBuilder.Append(id.ToString());
            urlBuilder.Append("&");
            urlBuilder.Length--;
            var url = urlBuilder.ToString();
            using (var request = new HttpRequestMessage(new HttpMethod("DELETE"), new Uri(url, UriKind.RelativeOrAbsolute)))
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
        
        public async Task<OperationResult<Student>> GetStudentAsync(int id)
        {
            var urlBuilder = GetUrl("/api/Student");
            urlBuilder.Append("?");
            urlBuilder.Append(id.ToString());
            urlBuilder.Append("&");
            urlBuilder.Length--;
            var url = urlBuilder.ToString();
            using (var request = new HttpRequestMessage(new HttpMethod("GET"), new Uri(url, UriKind.RelativeOrAbsolute)))
			{
				var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                var status = (int)response.StatusCode;
                if (response.IsSuccessStatusCode)
				{
                    try {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonSerializer.Deserialize<Student>(content);
                        return OperationResult.Ok(result);
                    }
                    catch(Exception ex)
                    {
                        var message = $"Error in response. Status {status}. Error: {ex.Message}";
                        _logger.LogError(message);
                        return OperationResult.Fail<Student>(message);
                    }
				}
                else 
                {
                    return OperationResult.Fail<Student>($"Error - Status {status}");
                }
			}
        }
        
        public async Task<OperationResult<Student>> PostStudentAsync(Student student)
        {
            var url = GetUrl("/api/Student").ToString();
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), new Uri(url, UriKind.RelativeOrAbsolute)))
			{
                var body = JsonSerializer.Serialize(student);
				request.Content = new StringContent(body, Encoding.UTF8, "application/json");
				var response = await _httpClient.SendAsync(request);
                var status = (int)response.StatusCode;
                if (response.IsSuccessStatusCode)
				{
                    try {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonSerializer.Deserialize<Student>(content);
					    return OperationResult.Ok(result);
                    }
                    catch(Exception ex)
                    {
                        var message = $"Error in response. Status {status}. Error: {ex.Message}";
                        _logger.LogError(message);
                        return OperationResult.Fail<Student>(message);
                    }
				}
                else 
                {
                    return OperationResult.Fail<Student>($"Error - Status {status}");
                }
			}
        }
        
        public async Task<OperationResult<Student>> PutStudentAsync(Student student)
        {
            var url = GetUrl("/api/Student").ToString();
            using (var request = new HttpRequestMessage(new HttpMethod("PUT"), new Uri(url, UriKind.RelativeOrAbsolute)))
			{
                var body = JsonSerializer.Serialize(student);
				request.Content = new StringContent(body, Encoding.UTF8, "application/json");
				var response = await _httpClient.SendAsync(request);
                var status = (int)response.StatusCode;
                if (response.IsSuccessStatusCode)
				{
                    try {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonSerializer.Deserialize<Student>(content);
					    return OperationResult.Ok(result);
                    }
                    catch(Exception ex)
                    {
                        var message = $"Error in response. Status {status}. Error: {ex.Message}";
                        _logger.LogError(message);
                        return OperationResult.Fail<Student>(message);
                    }
				}
                else 
                {
                    return OperationResult.Fail<Student>($"Error - Status {status}");
                }
			}
        }
        
        public async Task<OperationResult> DeleteStudentAsync(int id)
        {
            var urlBuilder = GetUrl("/api/Student");
            urlBuilder.Append("?");
            urlBuilder.Append(id.ToString());
            urlBuilder.Append("&");
            urlBuilder.Length--;
            var url = urlBuilder.ToString();
            using (var request = new HttpRequestMessage(new HttpMethod("DELETE"), new Uri(url, UriKind.RelativeOrAbsolute)))
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