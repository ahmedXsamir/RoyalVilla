using RoyalVilla.DTO;

namespace RoyalVillaWeb.Services.IServices
{
    // Interface for Villa service operations
    public interface IVillaService
    {
        Task<T?> GetAllAsync<T>();
        Task<T?> GetAsync<T>(int id, string tokent);
        Task<T?> CreateAsync<T>(VillaCreateDTO dto, string tokent);
        Task<T?> UpdateAsync<T>(VillaUpdateDTO dto, string tokent);
        Task<T?> DeleteAsync<T>(int id, string tokent);

    }
}
