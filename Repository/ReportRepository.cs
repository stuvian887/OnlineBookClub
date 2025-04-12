using OnlineBookClub.Models;
using System.Runtime.CompilerServices;

namespace OnlineBookClub.Repository
{
    public class ReportRepository
    {
        private readonly OnlineBookClubContext _context;
        private readonly PlanMemberRepository _memberRepository;
        private readonly LearnRepository _learnRepository;
        public ReportRepository (OnlineBookClubContext context ,PlanMemberRepository memberRepository, LearnRepository learnRepository)
        {
            _context = context;
            _memberRepository = memberRepository;
            _learnRepository = learnRepository;
        }
        public async Task<(Post_Report,string message)> GetReplyReport(int UserId, int PlanId)
        {
            var User = await _memberRepository.GetUserRoleAsync(UserId, PlanId);
            if (User != "組長")
            {
                return (null, "你不是組長");
            }
            return (null,"");
        }
    }
}
