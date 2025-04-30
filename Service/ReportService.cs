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
        public async Task<ReportPageResultDTO> GetAllUnifiedReportsPaged(int userId, int planId, string keyword, ForPaging paging)
        {
            var postReports = await _repository.GetAllPostReport(userId, planId) ?? new List<Post_ReportDTO>();
            var replyReports = await _repository.GetAllReplyReport(userId, planId) ?? new List<Reply_ReportDTO>();

            var unified = postReports.Select(r => new UnifiedReportDTO
            {
                ReportId = r.P_Report_Id,
                TargetId = r.Post_Id,
                Type = "Post",
                Action = r.Action,
                ReportText = r.Report_text,
                ReportTime = r.Report_Time
            })
            .Concat(replyReports.Select(r => new UnifiedReportDTO
            {
                ReportId = r.R_Report_Id,
                TargetId = r.Reply_Id,
                Type = "Reply",
                Action = r.Action,
                ReportText = r.Report_text,
                ReportTime = r.Report_Time
            }));

            // 關鍵字篩選
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                unified = unified.Where(r => r.ReportText.Contains(keyword)
                                            || r.Action.Contains(keyword)||
                                            r.Type.Contains(keyword));
            }

            // 分頁處理
            int total = unified.Count();
            paging.MaxPage = (int)Math.Ceiling((double)total / paging.ItemNum);
            paging.SetRightPage();

            var pagedResult = unified
                .OrderByDescending(r => r.ReportTime)
                .Skip((paging.NowPage - 1) * paging.ItemNum)
                .Take(paging.ItemNum)
                .ToList();

            return new ReportPageResultDTO
            {
                Keyword = keyword,
                Reports = pagedResult,
                Paging = paging
            };
        }


    }
}
