using Microsoft.EntityFrameworkCore;
using OnlineBookClub.DTO;
using OnlineBookClub.Models;

namespace OnlineBookClub.Repository
{
    public class StatisticRepository
    {
        private readonly OnlineBookClubContext _context;
        public StatisticRepository(OnlineBookClubContext context)
        {
            _context = context;
        }
        public IQueryable<Statistic> GetAll()
        {
            return _context.Statistic.AsQueryable();
        }
        public async Task<Statistic?> GetByPlanIdAsync(int planId)
        {
            return await _context.Statistic.FirstOrDefaultAsync(s => s.Plan_Id == planId);
        }

        public async Task AddAsync(Statistic stat)
        {
            await _context.Statistic.AddAsync(stat);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
