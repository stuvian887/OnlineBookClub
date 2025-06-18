using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using OnlineBookClub.Services;
using System.Runtime.CompilerServices;

namespace OnlineBookClub.Repository
{
    public class ReportRepository
    {
        private readonly OnlineBookClubContext _context;
        private readonly PlanMemberRepository _memberRepository;
        private readonly NoticeService _noticeService;
        public ReportRepository(OnlineBookClubContext context, PlanMemberRepository memberRepository, NoticeService noticeService)
        {
            _context = context;
            _memberRepository = memberRepository;
            _noticeService = noticeService;
        }
        public async Task<IEnumerable<Post_ReportDTO>> GetAllPostReport(int UserId, int PlanId)
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
                             Report_text = report.Report_text,
                             Report_Time = report.Report_Time,
                         };

            return await result.ToListAsync();
        }
        public async Task<IEnumerable<Reply_ReportDTO>> GetAllReplyReport(int UserId, int PlanId)
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
                             Report_text = report.Report_text,
                             Report_Time = report.Report_Time,
                         };

            return await result.ToListAsync();
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
                Report_text = reportWithPost.Report.Report_text,
                Report_Time = reportWithPost.Report.Report_Time
            };
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
                Report_text = reportWithReply.Report.Report_text,
                Report_Time = reportWithReply.Report.Report_Time,
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
                Report_Time = DateTime.Now.Date,
            };
            await _context.Post_Report.AddAsync(postReport);
            await _context.SaveChangesAsync();
            Post_ReportDTO resultDTO = new Post_ReportDTO
            {
                Report_text = postReport.Report_text,
                Report_Time = postReport.Report_Time,                
            };
            return (resultDTO, "貼文檢舉成功");
        }
        public async Task<Post_ReportDTO> DoPost_ReportActionAsync(int UserId , int P_Report_Id , [FromBody]Post_ReportDTO postreportAction)
        {
            var report = await _context.Post_Report
            .Include(r => r.Post)
            .FirstOrDefaultAsync(r => r.P_Report_Id == P_Report_Id);

            if (report == null)
            {
                return null;
            }

            var isLeader = await _context.PlanMembers.AnyAsync(pm =>
                pm.User_Id == UserId &&
                pm.Plan_Id == report.Post.Plan_Id &&
                pm.Role == "組長");

            if (!isLeader)
            {
                return null;
            }
            var post = report.Post;
            var booklan = await _context.BookPlan.Where(b => b.Plan_Id == post.Plan_Id).FirstOrDefaultAsync();
            report.Action = postreportAction.Action; 
            _context.Post_Report.Update(report);
            if (report.Action == "移除")
            {
                
                    var notification = new Notice
                    {

                        User_Id = post.User_Id,  // 通知給原貼文作者
                        NoticeTime = DateTime.Now,
                        Message = $"您在{booklan.Plan_Name}計畫的貼文{post.Content} 因 {postreportAction.Report_text} 原因已遭組長認證刪除",
                        User = post.User,
                    };
                    await _noticeService.CreateNoticeAsync(notification);  // 保存通知到資料庫
                    await _noticeService.GetNoticesByUserIdAsync(post.User_Id);
                
            }
            await _context.SaveChangesAsync();

            return new Post_ReportDTO
            {
                P_Report_Id = report.P_Report_Id,
                Post_Id = report.Post_Id,
                Action = report.Action,
                Report_text = report.Report_text,
                Report_Time = report.Report_Time
            };
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
                Report_text = RRData.Report_text,
                Report_Time = DateTime.Now.Date,
            };
            await _context.Reply_Report.AddAsync(reply_Report);
            await _context.SaveChangesAsync();
            Reply_ReportDTO resultDTO = new Reply_ReportDTO
            {
                Report_text = reply_Report.Report_text,
                Report_Time = reply_Report.Report_Time,
            };
            return (resultDTO, "留言檢舉成功");

        }

        public async Task<Reply_ReportDTO> DoReply_ReportActionAsync(int UserId, int R_Report_Id, [FromBody] Reply_ReportDTO replyreportAction)
        {
            var report = await _context.Reply_Report
            .Include(r => r.Reply)
            .ThenInclude(rp => rp.Post)
            .FirstOrDefaultAsync(r => r.R_Report_Id == R_Report_Id);

            if (report == null)
            {
                return null;
            }

            if(report.Reply == null)
            {
                return null;
            }
            var PlanId = report.Reply?.Post?.Plan_Id;
            if(PlanId == null)
            {
                return null;
            }

            var isLeader = await _context.PlanMembers.AnyAsync(pm =>
                pm.User_Id == UserId &&
                pm.Plan_Id == PlanId &&
                pm.Role == "組長");


            if (!isLeader)
            {
                return null;
            }

            report.Action = replyreportAction.Action;
            _context.Reply_Report.Update(report);
            var Reply = report.Reply;
            var post = await _context.Post.Where(b => b.Post_Id == Reply.Post_Id).FirstOrDefaultAsync();
            var bookplan = await _context.BookPlan.Where(p => p.Plan_Id == post.Plan_Id).FirstOrDefaultAsync();
            if (report.Action == "移除")
            {
                var notification = new Notice
                {

                    User_Id = Reply.User_Id,  // 通知給原貼文作者
                    NoticeTime = DateTime.Now,
                    Message = $"您在{bookplan.Plan_Name}計畫的回覆留言{Reply.ReplyContent} 因 {replyreportAction.Report_text} 原因已遭組長認證刪除",
                    User = Reply.User,
                };
                await _noticeService.CreateNoticeAsync(notification);  // 保存通知到資料庫
                await _noticeService.GetNoticesByUserIdAsync(Reply.User_Id);
            }
            await _context.SaveChangesAsync();

            return new Reply_ReportDTO
            {
                R_Report_Id = report.R_Report_Id,
                Reply_Id = report.Reply_Id,
                Action = report.Action,
                Report_text = report.Report_text,
                Report_Time = report.Report_Time
            };
        }
    }
}
