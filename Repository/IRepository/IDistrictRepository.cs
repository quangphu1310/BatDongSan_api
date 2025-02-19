using BatDongSan_api.Models;

namespace BatDongSan_api.Repository.IRepository
{
    public interface IDistrictRepository : IRepository<District>
    {
        Task<District> UpdateAsync(District entity);
        Task<District> CreateAsync(District entity);
    }
}
