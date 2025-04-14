using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using OnlineBookClub.Repository;
using System.Runtime.CompilerServices;

namespace OnlineBookClub.Service
{
    public class ReportService
    {
        private readonly ReportRepository _repository;
        public ReportService(ReportRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<Post_ReportDTO>> GetPostReport(int UserId , int PlanId , int PostId)
        {
            return await _repository.GetPostReport(UserId, PlanId, PostId); 
        }
        public async Task<IEnumerable<Reply_ReportDTO>> GetReplyReport(int UserId , int PlanId , int PostId , int ReplyId)
        {
            return await _repository.GetReplyReport(UserId , PlanId , PostId, ReplyId);
        }
        public async Task<(Post_ReportDTO, string Message)> CreatePostReport(int UserId, int PlanId, int PostId, Post_ReportDTO PRData)
        {
            return await _repository.CreatePostReport(UserId, PlanId, PostId, PRData);
        }
        public async Task<(Reply_ReportDTO,string Message)> CreateReplyReport(int UserId , int PlanId , int PostId , int ReplyId , Reply_ReportDTO RRData)
        {
            return await _repository.CreateReplyReport(UserId , PlanId , PostId , ReplyId , RRData);
        }
    }
}
