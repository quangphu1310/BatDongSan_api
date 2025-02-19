using BatDongSan_api.Models;

namespace BatDongSan_api.Repository.IRepository
{
    public interface IProvinceRepository : IRepository<Province>
    {
        Task<Province> UpdateAsync(Province entity);
        Task<Province> CreateAsync(Province entity);
    }
}
