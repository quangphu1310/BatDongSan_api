
using BatDongSan_api.Data;
using BatDongSan_api.Models;
using BatDongSan_api.Repository.IRepository;

namespace BatDongSan_api.Repository
{
    public class ProvinceRepository : Repository<Province>, IProvinceRepository
    {
        private readonly ApplicationDbContext _db;

        public ProvinceRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Province> CreateAsync(Province entity)
        {
            _db.Provinces.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<Province> UpdateAsync(Province entity)
        {
            _db.Provinces.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}
