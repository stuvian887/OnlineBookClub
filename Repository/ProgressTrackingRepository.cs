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
        public async Task<Learn> GetProgressTrack(int LearnId)
        {
            return await _context.Learn.FindAsync(LearnId);
        }
    }
}
