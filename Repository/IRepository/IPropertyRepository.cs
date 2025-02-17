using BatDongSan_api.Models;
using Microsoft.EntityFrameworkCore;

namespace BatDongSan_api.Repository.IRepository
{
    public interface IPropertyRepository : IRepository<Property>
    {
        Task<Property> UpdateAsync(Property entity);
        Task<Property> CreateAsync(Property entity);
        IQueryable<Property> GetAllAsQueryable();
    }
}
