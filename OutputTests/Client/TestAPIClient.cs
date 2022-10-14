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

        public async Task<OperationResult<Course>> GetCourseAsync(int id)
        {
            var urlBuilder = GetUrl("/api/Course");
            urlBuilder.Append("?");
            urlBuilder.Append("id=");
            urlBuilder.Append(id.ToString());
            
            
            var url = urlBuilder.ToString().ToLower();
            try
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), new Uri(url, UriKind.RelativeOrAbsolute)))
			    {
				    var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                    var status = (int)response.StatusCode;
                    if (response.IsSuccessStatusCode)
				    {
                        try {
                            var content = await response.Content.ReadAsStringAsync();
                            var result = JsonSerializer.Deserialize<Course>(content, _options);
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
            catch (Exception ex)
            {
                var message = $"Error - Get.GetCourseAsync - {ex.Message}";
                _logger.LogError(message);
                return OperationResult.Fail<Course>(message);
            }
        }
        
        public async Task<OperationResult<Course>> PostCourseAsync(Course course)
        {
            var url = GetUrl("/api/Course").ToString().ToLower();
            try
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), new Uri(url, UriKind.RelativeOrAbsolute)))
			    {
                    var body = JsonSerializer.Serialize(course, _options);
				    request.Content = new StringContent(body, Encoding.UTF8, "application/json");
				    var response = await _httpClient.SendAsync(request);
                    var status = (int)response.StatusCode;
                    if (response.IsSuccessStatusCode)
				    {
                        try {
                            var content = await response.Content.ReadAsStringAsync();
                            var result = JsonSerializer.Deserialize<Course>(content, _options);
					        return OperationResult.Ok(result);
                        }
                        catch(Exception ex)
                        {
                            var message = $"Error in response. Status {status}. Error - Post.PostCourseAsync - {ex.Message}";
                            _logger.LogError(message);
                            return OperationResult.Fail<Course>(message);
                        }
				    }
                    else
                    {
                        var message = $"Warning - Unsuccessful Http Result - Post.PostCourseAsync - Status:{status}";
                        _logger.LogWarning(message);
                        return OperationResult.Fail<Course>(message);
                    }
			    }
            }
            catch (Exception ex)
            {
                var message = $"Error - Post.PostCourseAsync - {ex.Message}";
                _logger.LogError(message);
                return OperationResult.Fail<Course>(message);
            }
        }
        
        public async Task<OperationResult<Course>> PutCourseAsync(Course course)
        {
            var url = GetUrl("/api/Course").ToString().ToLower();
            try
            {
                using (var request = new HttpRequestMessage(new HttpMethod("PUT"), new Uri(url, UriKind.RelativeOrAbsolute)))
			    {
                    var body = JsonSerializer.Serialize(course, _options);
				    request.Content = new StringContent(body, Encoding.UTF8, "application/json");
				    var response = await _httpClient.SendAsync(request);
                    var status = (int)response.StatusCode;
                    if (response.IsSuccessStatusCode)
				    {
                        try {
                            var content = await response.Content.ReadAsStringAsync();
                            var result = JsonSerializer.Deserialize<Course>(content, _options);
					        return OperationResult.Ok(result);
                        }
                        catch(Exception ex)
                        {
                            var message = $"Error in response. Status {status}. Error - Put.PutCourseAsync - {ex.Message}";
                            _logger.LogError(message);
                            return OperationResult.Fail<Course>(message);
                        }
				    }
                    else
                    {
                        var message = $"Warning - Unsuccessful Http Result - Put.PutCourseAsync - Status:{status}";
                        _logger.LogWarning(message);
                        return OperationResult.Fail<Course>(message);
                    }
			    }
            }
            catch (Exception ex)
            {
                var message = $"Error - Put.PutCourseAsync - {ex.Message}";
                _logger.LogError(message);
                return OperationResult.Fail<Course>(message);
            }
        }
        
        public async Task<OperationResult> DeleteCourseAsync(int id)
        {
            var urlBuilder = GetUrl("/api/Course");
            urlBuilder.Append("?");
            urlBuilder.Append("id=");
            urlBuilder.Append(id.ToString());
            
            var url = urlBuilder.ToString();
            try
            {
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
            catch (Exception ex)
            {
                var message = $"Error - Delete.DeleteCourseAsync - {ex.Message}";
                _logger.LogError(message);
                return OperationResult.Fail(message);
            }            
        }
        
        public async Task<OperationResult<Student>> GetStudentAsync(int id)
        {
            var urlBuilder = GetUrl("/api/Student");
            urlBuilder.Append("?");
            urlBuilder.Append("id=");
            urlBuilder.Append(id.ToString());
            
            
            var url = urlBuilder.ToString().ToLower();
            try
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), new Uri(url, UriKind.RelativeOrAbsolute)))
			    {
				    var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                    var status = (int)response.StatusCode;
                    if (response.IsSuccessStatusCode)
				    {
                        try {
                            var content = await response.Content.ReadAsStringAsync();
                            var result = JsonSerializer.Deserialize<Student>(content, _options);
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
            catch (Exception ex)
            {
                var message = $"Error - Get.GetStudentAsync - {ex.Message}";
                _logger.LogError(message);
                return OperationResult.Fail<Student>(message);
            }
        }
        
        public async Task<OperationResult<Student>> PostStudentAsync(Student student)
        {
            var url = GetUrl("/api/Student").ToString().ToLower();
            try
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), new Uri(url, UriKind.RelativeOrAbsolute)))
			    {
                    var body = JsonSerializer.Serialize(student, _options);
				    request.Content = new StringContent(body, Encoding.UTF8, "application/json");
				    var response = await _httpClient.SendAsync(request);
                    var status = (int)response.StatusCode;
                    if (response.IsSuccessStatusCode)
				    {
                        try {
                            var content = await response.Content.ReadAsStringAsync();
                            var result = JsonSerializer.Deserialize<Student>(content, _options);
					        return OperationResult.Ok(result);
                        }
                        catch(Exception ex)
                        {
                            var message = $"Error in response. Status {status}. Error - Post.PostStudentAsync - {ex.Message}";
                            _logger.LogError(message);
                            return OperationResult.Fail<Student>(message);
                        }
				    }
                    else
                    {
                        var message = $"Warning - Unsuccessful Http Result - Post.PostStudentAsync - Status:{status}";
                        _logger.LogWarning(message);
                        return OperationResult.Fail<Student>(message);
                    }
			    }
            }
            catch (Exception ex)
            {
                var message = $"Error - Post.PostStudentAsync - {ex.Message}";
                _logger.LogError(message);
                return OperationResult.Fail<Student>(message);
            }
        }
        
        public async Task<OperationResult<Student>> PutStudentAsync(Student student)
        {
            var url = GetUrl("/api/Student").ToString().ToLower();
            try
            {
                using (var request = new HttpRequestMessage(new HttpMethod("PUT"), new Uri(url, UriKind.RelativeOrAbsolute)))
			    {
                    var body = JsonSerializer.Serialize(student, _options);
				    request.Content = new StringContent(body, Encoding.UTF8, "application/json");
				    var response = await _httpClient.SendAsync(request);
                    var status = (int)response.StatusCode;
                    if (response.IsSuccessStatusCode)
				    {
                        try {
                            var content = await response.Content.ReadAsStringAsync();
                            var result = JsonSerializer.Deserialize<Student>(content, _options);
					        return OperationResult.Ok(result);
                        }
                        catch(Exception ex)
                        {
                            var message = $"Error in response. Status {status}. Error - Put.PutStudentAsync - {ex.Message}";
                            _logger.LogError(message);
                            return OperationResult.Fail<Student>(message);
                        }
				    }
                    else
                    {
                        var message = $"Warning - Unsuccessful Http Result - Put.PutStudentAsync - Status:{status}";
                        _logger.LogWarning(message);
                        return OperationResult.Fail<Student>(message);
                    }
			    }
            }
            catch (Exception ex)
            {
                var message = $"Error - Put.PutStudentAsync - {ex.Message}";
                _logger.LogError(message);
                return OperationResult.Fail<Student>(message);
            }
        }
        
        public async Task<OperationResult> DeleteStudentAsync(int id)
        {
            var urlBuilder = GetUrl("/api/Student");
            urlBuilder.Append("?");
            urlBuilder.Append("id=");
            urlBuilder.Append(id.ToString());
            
            var url = urlBuilder.ToString();
            try
            {
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
            catch (Exception ex)
            {
                var message = $"Error - Delete.DeleteStudentAsync - {ex.Message}";
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