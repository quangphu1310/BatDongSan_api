using BatDongSan_api.Data;
using BatDongSan_api.Models;
using BatDongSan_api.Repository.IRepository;

namespace BatDongSan_api.Repository
{
    public class DistrictRepository : Repository<District>, IDistrictRepository
    {
        private readonly ApplicationDbContext _db;

        public DistrictRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<District> CreateAsync(District entity)
        {
            _db.Districts.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<District> UpdateAsync(District entity)
        {
            _db.Districts.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}
