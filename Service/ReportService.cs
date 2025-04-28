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
        public async Task<Post_ReportDTO> GetPostReport(int UserId , int P_Report_Id)
        {
            return await _repository.GetPostReportAsync(UserId , P_Report_Id); 
        }
        public async Task<IEnumerable<Post_ReportDTO>> GetAllPostReport(int UserId , int PlanId)
        {
            return await _repository.GetAllPostReport(UserId , PlanId);
        }
        public async Task<IEnumerable<Reply_ReportDTO>> GetAllReplyReport(int UserId , int PlanId)
        {
            return await _repository.GetAllReplyReport(UserId, PlanId);
        }
        public async Task<Reply_ReportDTO> GetReplyReport(int UserId , int R_Report_Id)
        {
            return await _repository.GetReplyReportAsync(UserId , R_Report_Id);
        }
        public async Task<(Post_ReportDTO, string Message)> CreatePostReport(int UserId, int PlanId, int PostId, Post_ReportDTO PRData)
        {
            return await _repository.CreatePostReport(UserId, PlanId, PostId, PRData);
        }
        public async Task<Post_ReportDTO> DoPost_ReportAction(int UserId , int P_Report_Id , Post_ReportDTO DoingAction)
        {
            return await _repository.DoPost_ReportActionAsync(UserId , P_Report_Id , DoingAction);
        }
        public async Task<Reply_ReportDTO> DoReply_ReportAction(int UserId , int R_Report_Id , Reply_ReportDTO DoingAction)
        {
            return await _repository.DoReply_ReportActionAsync(UserId, R_Report_Id, DoingAction);
        }
        public async Task<(Reply_ReportDTO,string Message)> CreateReplyReport(int UserId , int PlanId , int PostId , int ReplyId , Reply_ReportDTO RRData)
        {
            return await _repository.CreateReplyReport(UserId , PlanId , PostId , ReplyId , RRData);
        }
    }
}
