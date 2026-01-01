using RoyalVilla.DTO;
using RoyalVillaWeb.Models;

namespace RoyalVillaWeb.Services.IServices
{
    public interface IBaseService
    {
        Task<T> SendAsync<T>(APIRequest apiRequest);
        APIResponse<object> ResponseModel { get; set; }
    }
}
