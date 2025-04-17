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
        public async Task<IEnumerable<Post_ReportDTO>> GetPostReport(int UserId ,int PlanId, int PostId)
        {
            var User = await _memberRepository.GetUserRoleAsync(UserId, PlanId);
            if (User == "組長")
            {
                var bookPlan = await _context.BookPlan.FindAsync(PlanId);
                if(bookPlan == null)
                {
                    return null;
                }
                var post = await _context.Post.FindAsync(PostId);
                if(post == null && bookPlan.Plan_Id == post.Plan_Id)
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
        public async Task<IEnumerable<Reply_ReportDTO>> GetReplyReport(int UserId , int PlanId , int PostId , int ReplyId)
        {
            var User = await _memberRepository.GetUserRoleAsync(UserId,PlanId);
            if(User == "組長")
            {
                var bookPlan = await _context.BookPlan.FindAsync(PlanId);
                if(bookPlan != null)
                {
                    var post = await _context.Post.FindAsync(PostId);
                    if (post != null && bookPlan.Plan_Id == post.Plan_Id)
                    {
                        var reply = await _context.Reply.FindAsync(ReplyId);
                        if (reply != null && post.Post_Id == reply.Post_Id)
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
                    else { return null; }
                }
                else { return null;}
            }
            else { return null;  }
        }
        public async Task<(Post_ReportDTO, string message)> CreatePostReport(int UserId, int PlanId, int PostId, Post_ReportDTO PRData)
        {
            var User = await _memberRepository.GetUserRoleAsync(UserId, PlanId);
            if (User != null)
            {
                var bookPlan = await _context.BookPlan.FindAsync(PlanId);
                if (bookPlan != null)
                {
                    var post = await _context.Post.FindAsync(PostId);
                    if (post != null && bookPlan.Plan_Id == post.Plan_Id)
                    {
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
                    else { return (null, "此貼文不在此計畫中或貼文不存在"); }
                }
                else { return (null,"此計畫不存在"); }
            }
            else { return (null, "你不是這個計畫的成員");}
        }
        public async Task<(Reply_ReportDTO, string message)> CreateReplyReport(int UserId, int PlanId, int PostId,int ReplyId, Reply_ReportDTO RRData)
        {
            var User = await _memberRepository.GetUserRoleAsync(UserId, PlanId);
            if (User != null)
            {
                var bookPlan = await _context.BookPlan.FindAsync(PlanId);
                if (bookPlan != null)
                {
                    var post = await _context.Post.FindAsync(PostId);
                    if (post != null && bookPlan.Plan_Id == post.Plan_Id)
                    {
                        var reply = await _context.Reply.FindAsync(ReplyId);
                        if (reply != null && post.Post_Id == reply.Post_Id)
                        {
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
                        else { return (null, "此留言不在此貼文或貼文不存在"); }
                    }
                    else { return (null, "此貼文不再該計畫或計畫不存在"); }
                }
                else { return (null, "找不到此計畫"); }
            }
            else { return (null, "你不是這個計畫的成員"); }
        }
    }
}
