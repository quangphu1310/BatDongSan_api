using BatDongSan_api.Models;

namespace BatDongSan_api.Repository.IRepository
{
    public interface IPropertyImageRepository : IRepository<PropertyImage>
    {
        Task<PropertyImage> UpdateAsync(PropertyImage entity);
        Task<PropertyImage> CreateAsync(PropertyImage entity);
    }
}
