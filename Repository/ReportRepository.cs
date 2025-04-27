using Microsoft.EntityFrameworkCore;
using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using System.Runtime.CompilerServices;

namespace OnlineBookClub.Repository
{
    public class ReportRepository
    {
        private readonly OnlineBookClubContext _context;
        private readonly PlanMemberRepository _memberRepository;
        public ReportRepository(OnlineBookClubContext context, PlanMemberRepository memberRepository)
        {
            _context = context;
            _memberRepository = memberRepository;
        }
        public async Task<IEnumerable<Post_ReportDTO>> GetPostReport(int UserId, int PlanId, int PostId)
        {
            var User = await _memberRepository.GetUserRoleAsync(UserId, PlanId);
            if (User == "組長")
            {
                var bookPlan = await _context.BookPlan.FindAsync(PlanId);
                if (bookPlan == null)
                {
                    return null;
                }
                var post = await _context.Post.FindAsync(PostId);
                if (post == null && bookPlan.Plan_Id == post.Plan_Id)
                {
                    return null;
                }
                var result = (from a in _context.Post_Report
                                          .Where(a => a.Post_Id == PostId)
                              select new Post_ReportDTO
                              {
                                  P_Report_Id = a.P_Report_Id,
                                  Post_Id = a.Post_Id,
                                  Action = a.Action,
                                  Report_text = a.Report_text
                              });
                return result;
            }
            else { return null; }
        }
        public async Task<IEnumerable<Post_ReportDTO>> GetAllPostReport(int UserId)
        {
            var leaderPlanIds = await _context.PlanMembers
            .Where(pm => pm.User_Id == UserId && pm.Role == "組長")
            .Select(pm => pm.Plan_Id)
            .ToListAsync();

            if (leaderPlanIds == null || leaderPlanIds.Count == 0)
                return null;

            var result = from report in _context.Post_Report
                         join post in _context.Post on report.Post_Id equals post.Post_Id
                         where leaderPlanIds.Contains(post.Plan_Id)
                         select new Post_ReportDTO
                         {
                             P_Report_Id = report.P_Report_Id,
                             Post_Id = report.Post_Id,
                             Action = report.Action,
                             Report_text = report.Report_text
                         };

            return await result.ToListAsync();
        }
        public async Task<IEnumerable<Reply_ReportDTO>> GetAllReplyReport(int UserId)
        {
            var leaderPlanIds = await _context.PlanMembers
            .Where(pm => pm.User_Id == UserId && pm.Role == "組長")
            .Select(pm => pm.Plan_Id)
            .ToListAsync();

            if (leaderPlanIds == null || leaderPlanIds.Count == 0)
                return null;

            var result = from report in _context.Reply_Report
                         join reply in _context.Reply on report.Reply_Id equals reply.Reply_Id
                         join post in _context.Post on reply.Post_Id equals post.Post_Id
                         where leaderPlanIds.Contains(post.Plan_Id)
                         select new Reply_ReportDTO
                         {
                             R_Report_Id = report.R_Report_Id,
                             Reply_Id = report.Reply_Id,
                             Action = report.Action,
                             Report_text = report.Report_text
                         };

            return await result.ToListAsync();
        }
        public async Task<IEnumerable<Reply_ReportDTO>> GetReplyReport(int UserId, int PlanId, int PostId, int ReplyId)
        {
            var User = await _memberRepository.GetUserRoleAsync(UserId, PlanId);
            var bookPlan = await _context.BookPlan.FindAsync(PlanId);
            var post = await _context.Post.FindAsync(PostId);
            var reply = await _context.Reply.FindAsync(ReplyId);
            if (User == "組長" && bookPlan != null && (post != null && bookPlan.Plan_Id == post.Plan_Id) && (reply != null && post.Post_Id == reply.Post_Id))
            {
                var replys = (from a in _context.Reply_Report
                              .Where(a => a.Reply_Id == ReplyId)
                              select new Reply_ReportDTO
                              {
                                  R_Report_Id = a.R_Report_Id,
                                  Reply_Id = a.Reply_Id,
                                  Action = a.Action,
                                  Report_text = a.Report_text
                              });
                return replys;
            }
            else { return null; }
        }
        public async Task<(Post_ReportDTO, string message)> CreatePostReport(int UserId, int PlanId, int PostId, Post_ReportDTO PRData)
        {
            var User = await _memberRepository.GetUserRoleAsync(UserId, PlanId);
            if (User == null)
            {
                return (null, "你不是這個計畫的成員");
            }

            var bookPlan = await _context.BookPlan.FindAsync(PlanId);
            if (bookPlan == null)
            {
                return (null, "此計畫不存在");
            }

            var post = await _context.Post.FindAsync(PostId);
            if (post == null)
            {
                return (null, "貼文不存在");
            }

            if(bookPlan.Plan_Id != post.Plan_Id)
            {
                return (null, "此貼文不在此計畫中");
            }

            Post_Report postReport = new Post_Report
            {
                Post_Id = PostId,
                Report_text = PRData.Report_text,
            };
            await _context.Post_Report.AddAsync(postReport);
            await _context.SaveChangesAsync();
            Post_ReportDTO resultDTO = new Post_ReportDTO
            {
                Report_text = postReport.Report_text
            };
            return (resultDTO, "貼文檢舉成功");
        }
        public async Task<(Reply_ReportDTO, string message)> CreateReplyReport(int UserId, int PlanId, int PostId, int ReplyId, Reply_ReportDTO RRData)
        {
            var User = await _memberRepository.GetUserRoleAsync(UserId, PlanId);
            if (User == null)
            {
                return (null, "你不是這個計畫的成員");
            }

            var bookPlan = await _context.BookPlan.FindAsync(PlanId);
            if (bookPlan == null)
            {
                return (null, "找不到此計畫");
            }

            var post = await _context.Post.FindAsync(PostId);
            if (post == null)
            {
                return (null, "計畫不存在");
            }

            if (bookPlan.Plan_Id != post.Plan_Id)
            {
                return (null, "此貼文不在該計畫");
            }

            var reply = await _context.Reply.FindAsync(ReplyId);
            if (reply == null)
            {
                return (null, "貼文不存在");
            }

            if (post.Post_Id != reply.Post_Id)
            {
                return (null, "此留言不在此貼文");
            }

            Reply_Report reply_Report = new Reply_Report
            {
                Reply_Id = ReplyId,
                Report_text = RRData.Report_text
            };
            await _context.Reply_Report.AddAsync(reply_Report);
            await _context.SaveChangesAsync();
            Reply_ReportDTO resultDTO = new Reply_ReportDTO
            {
                Report_text = reply_Report.Report_text
            };
            return (resultDTO, "留言檢舉成功");

        }
    }
}
