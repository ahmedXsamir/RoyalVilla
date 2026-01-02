using RoyalVilla.DTO;
using RoyalVillaWeb.Models;
using RoyalVillaWeb.Services.IServices;
using System.Text.Json;

namespace RoyalVillaWeb.Services
{
    public class BaseService : IBaseService
    {
        public IHttpClientFactory _httpClient;

        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        public APIResponse<object> ResponseModel { get; set; }
        public BaseService(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;   
            ResponseModel = new APIResponse<object>();
        }

        public async Task<T?> SendAsync<T>(APIRequest apiRequest)
        {
            try
            {
                var client = _httpClient.CreateClient("RoyalVillaAPI");
                HttpRequestMessage message = new HttpRequestMessage()
                {
                    RequestUri = new Uri(apiRequest.Url,UriKind.Relative),
                    Method = apiRequest.ApiType switch
                    {
                        SD.APIType.POST => HttpMethod.Post,
                        SD.APIType.PUT => HttpMethod.Put,
                        SD.APIType.DELETE => HttpMethod.Delete,
                        _ => HttpMethod.Get
                    }
                };
                
                if (apiRequest.Data != null)
                {
                    // Serialize the data to JSON and add it to the request body
                    message.Content = JsonContent.Create(apiRequest.Data, options: jsonSerializerOptions);  
                }

                var apiResponse = await client.SendAsync(message);
                return await apiResponse.Content.ReadFromJsonAsync<T>(jsonSerializerOptions);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
                return default;
            }
        }

    }
}
