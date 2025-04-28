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
        public async Task<Post_ReportDTO> GetPostReportAsync(int userId, int P_ReportId)
        {
            var reportWithPost = await (from report in _context.Post_Report
                                        join post in _context.Post
                                        on report.Post_Id equals post.Post_Id
                                        where report.P_Report_Id == P_ReportId
                                        select new
                                        {
                                            Report = report,
                                            Post = post
                                        }).FirstOrDefaultAsync();
            if (reportWithPost == null)
            {
                return null;
            }
            var isLeader = await _context.PlanMembers.AnyAsync(pm =>
                pm.User_Id == userId &&
                pm.Plan_Id == reportWithPost.Post.Plan_Id &&
                pm.Role == "組長");

            if (!isLeader)
            {
                return null;
            }

            return new Post_ReportDTO
            {
                P_Report_Id = reportWithPost.Report.P_Report_Id,
                Post_Id = reportWithPost.Report.Post_Id,
                Action = reportWithPost.Report.Action,
                Report_text = reportWithPost.Report.Report_text
            };
        }

        public async Task<IEnumerable<Post_ReportDTO>> GetAllPostReport(int UserId , int PlanId)
        {
            var leaderPlanIds = await _context.PlanMembers
            .Where(pm => pm.User_Id == UserId && pm.Role == "組長" && pm.Plan_Id == PlanId)
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
        public async Task<IEnumerable<Reply_ReportDTO>> GetAllReplyReport(int UserId , int PlanId)
        {
            var leaderPlanIds = await _context.PlanMembers
            .Where(pm => pm.User_Id == UserId && pm.Role == "組長" && pm.Plan_Id == PlanId)
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
        public async Task<Reply_ReportDTO> GetReplyReportAsync(int UserId, int R_Report_Id)
        {
            var reportWithReply = await (from report in _context.Reply_Report
                                        join reply in _context.Reply
                                        on report.Reply_Id equals reply.Reply_Id
                                        join post in _context.Post
                                        on reply.Post_Id equals post.Post_Id
                                        where report.R_Report_Id == R_Report_Id
                                        select new
                                        {
                                            Report = report,
                                            Post = post,
                                            Reply = reply
                                        }).FirstOrDefaultAsync();
            if (reportWithReply == null)
            {
                return null;
            }
            var isLeader = await _context.PlanMembers.AnyAsync(pm =>
                pm.User_Id == UserId &&
                pm.Plan_Id == reportWithReply.Post.Plan_Id &&
                pm.Role == "組長");

            if (!isLeader)
            {
                return null;
            }

            return new Reply_ReportDTO
            {
                R_Report_Id = reportWithReply.Report.R_Report_Id,
                Reply_Id = reportWithReply.Report.Reply_Id,
                Action = reportWithReply.Report.Action,
                Report_text = reportWithReply.Report.Report_text
            };
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
