using BatDongSan_api.Data;
using BatDongSan_api.Models;
using BatDongSan_api.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BatDongSan_api.Repository
{
    public class PropertyRepository : Repository<Property>, IPropertyRepository
    {
        private readonly ApplicationDbContext _db;

        public PropertyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Property> CreateAsync(Property entity)
        {
            entity.PostedDate = DateTime.Now;
            _db.Properties.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<Property> UpdateAsync(Property entity)
        {
            entity.PostedDate = DateTime.Now;
            _db.Properties.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
        public IQueryable<Property> GetAllAsQueryable()
        {
            return _db.Properties.AsQueryable();
        }
    }
}
