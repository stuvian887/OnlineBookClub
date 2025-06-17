using Microsoft.EntityFrameworkCore;
using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using System;
using System.Diagnostics.Metrics;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OnlineBookClub.Repository
{
    public class BookPlanRepository
    {
        private readonly OnlineBookClubContext _context;
        private readonly PlanMemberRepository _planMemberRepsitory;

        public BookPlanRepository(OnlineBookClubContext context, PlanMemberRepository planMemberRepsitory , LearnRepository learnRepository)
        {
            _context = context;
            _planMemberRepsitory = planMemberRepsitory;
        }

        public async Task<List<BookPlanDTO>> GetPublicPlansBySearchAsync(int userid, string keyword, ForPaging paging, string order)
        {
            var query = from plan in _context.BookPlan
                        join member in _context.Members on plan.User_Id equals member.User_Id
                        join statistic in _context.Statistic on plan.Plan_Id equals statistic.Plan_Id into statGroup
                        from stat in statGroup.DefaultIfEmpty()
                        where plan.IsPublic && plan.User_Id != userid
                        select new
                        {
                            Plan = plan,
                            CreatorName = member.UserName,
                            Statistic = stat
                        };

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p =>
                    (p.Plan.Plan_Name ?? "").Contains(keyword) ||
                    (p.Plan.Plan_Goal ?? "").Contains(keyword) ||
                    (p.Plan.Plan_Type ?? "").Contains(keyword) ||
                    (p.CreatorName ?? "").Contains(keyword)
                );
            }

            int totalCount = await query.CountAsync();
            paging.MaxPage = (int)Math.Ceiling((double)totalCount / paging.ItemNum);
            paging.SetRightPage();

            // 排序條件處理
            switch (order?.ToLower())
            {
                case "view":
                    query = query.OrderByDescending(p => p.Statistic != null ? p.Statistic.ViewTimes : 0);
                    break;
                case "copy":
                    query = query.OrderByDescending(p => p.Statistic != null ? p.Statistic.CopyCount : 0);
                    break;
                case "user":
                    query = query.OrderByDescending(p => p.Statistic != null ? p.Statistic.UserCount : 0);
                    break;
                default:
                    query = query.OrderByDescending(p => p.Plan.Plan_Id); // 預設用 Plan_Id 倒序
                    break;
            }

            var pagedPlans = await query
                .Skip((paging.NowPage - 1) * paging.ItemNum)
                .Take(paging.ItemNum)
                .ToListAsync();

            var dtoList = new List<BookPlanDTO>();
            foreach (var p in pagedPlans)
            {
                (string recentlyLearnDate, string recentlyLearn) = await GetRecentlyLearn(p.Plan.Plan_Id);
                dtoList.Add(new BookPlanDTO
                {
                    Plan_ID = p.Plan.Plan_Id,
                    Plan_Name = p.Plan.Plan_Name,
                    Plan_Goal = p.Plan.Plan_Goal,
                    Plan_Type = p.Plan.Plan_Type,
                    Plan_Suject = p.Plan.Plan_suject,
                    IsPublic = p.Plan.IsPublic,
                    RecentlyLearnDate = recentlyLearnDate,
                    RecentlyLearn = recentlyLearn,
                    IsComplete = p.Plan.IsComplete,
                    CreatorName = p.CreatorName
                });
            }

            return dtoList;
        }


        public async Task<(string,string)> GetRecentlyLearn(int PlanId)
        {
            string TempLearn = "";
            double TempTime = 99999999f;
            string LearnDate = "";
            var PlanOfLearns = await _context.Learn.Where(l => l.Plan_Id == PlanId).ToListAsync();
            if (PlanOfLearns != null)
            {
                foreach (var learns in PlanOfLearns)
                {
                    DateTime NowTime = DateTime.UtcNow.Date.ToLocalTime();
                    System.TimeSpan FindRecentlyLearnTime = learns.DueTime.Subtract(NowTime);
                    if (FindRecentlyLearnTime.TotalSeconds >= 0 && FindRecentlyLearnTime.TotalSeconds <= TempTime)
                    {
                        TempTime = FindRecentlyLearnTime.TotalSeconds;
                        LearnDate = learns.DueTime.ToString("yyyy/MM/dd");
                        TempLearn = learns.Learn_Name;
                    }
                }
                return (LearnDate,TempLearn);
            }
            else
            {
                return (LearnDate,null);
            }
        }

        public async Task<int> GetPublicPlansCountAsync(string keyword)
        {
            var query = _context.BookPlan.Where(p => p.IsPublic);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p =>
                    p.Plan_Name.Contains(keyword) ||
                    p.Plan_Goal.Contains(keyword));
            }

            return await query.CountAsync();
        }



        public async Task<BookPlanDTO?> GetById(int id)
        {
            var plan = await _context.BookPlan.FindAsync(id);
            if (plan == null) return null;

            var user = await _context.Members
                .Where(u => u.User_Id == plan.User_Id)
                .Select(u => u.UserName)
                .FirstOrDefaultAsync();

            (string recentlyLearnDate, string recentlyLearn) = await GetRecentlyLearn(plan.Plan_Id);

            return new BookPlanDTO
            {
                Plan_ID = plan.Plan_Id,
                Plan_Name = plan.Plan_Name,
                Plan_Goal = plan.Plan_Goal,
                Plan_Type = plan.Plan_Type,
                Plan_Suject = plan.Plan_suject,
                IsPublic = plan.IsPublic,
                RecentlyLearnDate = recentlyLearnDate,
                RecentlyLearn = recentlyLearn,
                IsComplete = plan.IsComplete,
                CreatorName = user ?? "未知使用者"
            };
        }


        public async Task<List<BookPlanDTO>> GetPlansWithCreatorNameByUserId(int userId, string keyword, ForPaging paging)
        {
            // 1. 找出自己創建的計畫
            var createdPlans = await _context.BookPlan
                .Where(p => p.User_Id == userId)
                .ToListAsync();

            // 2. 找出自己參與的計畫
            var joinedPlanIds = await _context.PlanMembers
                .Where(m => m.User_Id == userId)
                .Select(m => m.Plan_Id)
                .ToListAsync();

            var joinedPlans = await _context.BookPlan
                .Where(p => joinedPlanIds.Contains(p.Plan_Id))
                .ToListAsync();

            // 3. 合併 + 避免重複
            var allPlans = createdPlans.Concat(joinedPlans).Distinct().ToList();

            // 4. 如果有關鍵字就過濾（搜尋名稱或目標）
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                allPlans = allPlans
                    .Where(p => (p.Plan_Name?.Contains(keyword) ?? false)
                             || (p.Plan_Goal?.Contains(keyword) ?? false)
                             || (p.Plan_Type?.Contains(keyword) ?? false))
                    .ToList();
            }

            // 5. 分頁：先計算總數，設定 MaxPage，再取範圍資料
            paging.MaxPage = (int)Math.Ceiling((double)allPlans.Count / paging.ItemNum);
            paging.SetRightPage();

            var pagePlans = allPlans
                .OrderByDescending(p => p.Plan_Id)
                .Skip((paging.NowPage - 1) * paging.ItemNum)
                .Take(paging.ItemNum)
                .ToList();

            // 6. 查創建者名稱
            var creatorIds = pagePlans.Select(p => p.User_Id).Distinct().ToList();

            var users = await _context.Members
                .Where(u => creatorIds.Contains(u.User_Id))
                .ToDictionaryAsync(u => u.User_Id, u => u.UserName);

            // 7. 組成 DTO
            var dtoList = new List<BookPlanDTO>();
            foreach(var p in pagePlans)
            {
                (string recentlyLearnDate, string recentlylearn) = await GetRecentlyLearn(p.Plan_Id);
                dtoList.Add(new BookPlanDTO
                {
                    Plan_ID = p.Plan_Id,
                    Plan_Name = p.Plan_Name,
                    Plan_Goal = p.Plan_Goal,
                    Plan_Type = p.Plan_Type,
                    Plan_Suject = p.Plan_suject,
                    IsPublic = p.IsPublic,
                    RecentlyLearnDate = recentlyLearnDate,
                    RecentlyLearn = recentlylearn,
                    IsComplete = p.IsComplete,
                    CreatorName = users.ContainsKey(p.User_Id) ? users[p.User_Id] : "未知使用者"
                });
            }
            return dtoList;
        }
        public async Task<BookPlan> Create(BookPlan bookPlan)
        {

            _context.BookPlan.Add(bookPlan);
            await _context.SaveChangesAsync();
            await _planMemberRepsitory.AddUserToPlanAsync(bookPlan.User_Id,bookPlan.Plan_Id);
            return bookPlan;
        }
        public async Task<BookPlan?> GetEntityById(int id)
        {
            return await _context.BookPlan.FindAsync(id);
        }

        public async Task<BookPlan> Update(BookPlan bookPlan)
        {
            _context.BookPlan.Update(bookPlan);
            await _context.SaveChangesAsync();
            return bookPlan;
        }

        public async Task<bool> Delete(int PlanId)
        {
            var bookPlan = await _context.BookPlan
                                          .Include(bp => bp.PlanMembers)  // 確保加載相關的 PlanMembers
                                          .FirstOrDefaultAsync(bp => bp.Plan_Id == PlanId);
            var Learn = await _context.Learn
                             .Include(bp => bp.Topic)  // 確保加載相關的 PlanMembers
                             .FirstOrDefaultAsync(bp => bp.Plan_Id == PlanId);
            var pp = await _context.Learn
                             .Include(bp => bp.ProgressTracking)  // 確保加載相關的 PlanMembers
                             .FirstOrDefaultAsync(bp => bp.Learn_Id == Learn.Learn_Id);
            var book = await _context.BookPlan
                             .Include(bp => bp.Book)  // 確保加載相關的 PlanMembers
                             .FirstOrDefaultAsync(bp => bp.Plan_Id == bookPlan.Plan_Id);
            if (bookPlan == null)
                return false;
          
            // 先刪除相關的 PlanMembers
            _context.PlanMembers.RemoveRange(bookPlan.PlanMembers);
            
            
           
           
            // 然後刪除 BookPlans
            _context.BookPlan.Remove(bookPlan);

            // 保存變更到資料庫
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<List<BookPlanDTO>> GetMemberPlansByLeaderAsync(int leaderId, int memberId)
        {
            var joinedPlanIds = await _context.PlanMembers
                .Where(pm => pm.User_Id == memberId)
                .Select(pm => pm.Plan_Id)
                .ToListAsync();

            var plans = await _context.BookPlan
                .Where(p => joinedPlanIds.Contains(p.Plan_Id) && p.User_Id == leaderId)
                .ToListAsync();

            var userIds = plans.Select(p => p.User_Id).Distinct().ToList();
            var users = await _context.Members
                .Where(u => userIds.Contains(u.User_Id))
                .ToDictionaryAsync(u => u.User_Id, u => u.UserName);

            var dtoList = new List<BookPlanDTO>();
            foreach (var p in plans)
            {
                var JoinDateTime = await _context.PlanMembers
                    .Where(pl => pl.User_Id == memberId && pl.Plan_Id == p.Plan_Id && pl.Role == "組員")
                    .Select(pl => (DateTime)pl.JoinDate)
                    .FirstOrDefaultAsync();

                var LearnName = await _context.ProgressTracking
                    .Where(pt => pt.User_Id == memberId && pt.Learn.Plan_Id == p.Plan_Id && pt.Status == false)
                    .Join(_context.Learn,
                        progress => progress.Learn_Id,
                        learn => learn.Learn_Id,
                        (progress, learn) => new
                        {
                            Progress = progress,
                            Learn = learn,
                        }
                    )
                    .FirstOrDefaultAsync();

                var PassCount = await _context.ProgressTracking
                    .Where(pa => pa.User_Id == memberId && pa.Learn.Plan_Id == p.Plan_Id && pa.Status)
                    .CountAsync();

                var LearnCount = await _context.Learn
                    .Where(l => l.Plan_Id == p.Plan_Id)
                    .CountAsync();
                double PassPersent = Math.Ceiling(((double)PassCount / LearnCount) * 100);

                dtoList.Add(new BookPlanDTO
                {
                    Plan_ID = p.Plan_Id,
                    Plan_Name = p.Plan_Name,
                    Plan_Goal = p.Plan_Goal,
                    Plan_Type = p.Plan_Type,
                    Plan_Suject = p.Plan_suject,
                    IsPublic = p.IsPublic,
                    RecentlyLearnDate = LearnName.Progress.LearnDueTime.Date.ToString("yyyy/MM/dd"),
                    RecentlyLearn = LearnName.Learn.Learn_Name,
                    ProgressPercent = PassPersent.ToString() + "%",
                    JoinDate = JoinDateTime.Date.ToString("yyyy/MM/dd"),
                    IsComplete = p.IsComplete,
                    CreatorName = users.ContainsKey(p.User_Id) ? users[p.User_Id] : "未知使用者"
                });
            }

            return dtoList;
        }

        
    }
}
