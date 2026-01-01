using RoyalVilla.DTO;
using RoyalVillaWeb.Services.IServices;

namespace RoyalVillaWeb.Services
{
    public class VillaService : BaseService, IVillaService
    {
        private new readonly IHttpClientFactory _httpClient;
        public VillaService(IHttpClientFactory httpClient) : base(httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<T?> CreateAsync<T>(VillaCreateDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<T?> DeleteAsync<T>(int id)
        {
            throw new NotImplementedException();
        }

        public Task<T?> GetAllAsync<T>()
        {
            throw new NotImplementedException();
        }

        public Task<T?> GetAsync<T>(int id)
        {
            throw new NotImplementedException();
        }

        public Task<T?> UpdateAsync<T>(VillaUpdateDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
