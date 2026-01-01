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

        public Task<T> SendAsync<T>(APIRequest apiRequest)
        {
            throw new NotImplementedException();
        }
    }
}
