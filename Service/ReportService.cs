using OnlineBookClub.Models;
using OnlineBookClub.Repository;

namespace OnlineBookClub.Service
{
    public class ReportService
    {
        private readonly ReportRepository _repository;
        private readonly PlanMemberRepository _planMemberRepository;
        public ReportService(ReportRepository repository)
        {
            _repository = repository;
        }
        
    }
}
