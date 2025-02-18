using BatDongSan_api.Data;
using BatDongSan_api.Models;
using BatDongSan_api.Repository.IRepository;

namespace BatDongSan_api.Repository
{
    public class PropertyImageRepository : Repository<PropertyImage>, IPropertyImageRepository
    {
        private readonly ApplicationDbContext _db;

        public PropertyImageRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<PropertyImage> CreateAsync(PropertyImage entity)
        {
            _db.PropertyImages.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<PropertyImage> UpdateAsync(PropertyImage entity)
        {
            _db.PropertyImages.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}
