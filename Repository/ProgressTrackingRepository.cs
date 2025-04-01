using Microsoft.EntityFrameworkCore;
using OnlineBookClub.DTO;
using OnlineBookClub.Models;

namespace OnlineBookClub.Repository
{
    public class ProgressTrackingRepository
    {
        private readonly OnlineBookClubContext _context;
        public ProgressTrackingRepository(OnlineBookClubContext context)
        {
            _context = context;
        }
        public async Task<BookPlan> CheckLearnByPlanId(int PlanId)
        {
            return await _context.BookPlan.SingleOrDefaultAsync(b => b.Plan_Id == PlanId);
        }
        public async Task<Learn> CheckLearnByLearnId(int LearnId)
        {
            return await _context.Learn.SingleOrDefaultAsync(l => l.Learn_Id == LearnId);
        }   
    }
}
