using Microsoft.EntityFrameworkCore;
using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using OnlineBookClub.Service;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace OnlineBookClub.Repository
{
    public class PlanMemberRepository
    {
        private readonly OnlineBookClubContext _context;

        private readonly StatisticService _statisticService;
        [ActivatorUtilitiesConstructor]
        public PlanMemberRepository(OnlineBookClubContext context, StatisticService statisticService)
        {
            _context = context;
            _statisticService = statisticService;
        }

        public async Task<BookPlan> GetPlanByIdAsync(int planId)
        {
            return await _context.BookPlan.FirstOrDefaultAsync(p => p.Plan_Id == planId);
        }
        public async Task<int> GetPlanMemberTotal(int planid)
        {
            return await _context.PlanMembers.CountAsync(p => p.Plan_Id == planid);
        }
        
        public async Task<List<PlanMembersDTO>> PlanMembersInPlanAsync(int planid)
        {
            var members = await _context.PlanMembers
                .Where(pm => pm.Plan_Id == planid)
                .Join(_context.Members,
                      pm => pm.User_Id,
                      m => m.User_Id,
                      (pm, m) => new PlanMembersDTO
                      {
                          PlanId = pm.Plan_Id,
                          UserId = pm.User_Id,
                          Name = m.UserName,
                          Role=pm.Role
                      })
                .ToListAsync();

            return members;
        }

        public async Task<bool> IsUserInPlanAsync(int userId, int planId)
        {
            return await _context.PlanMembers.AnyAsync(pm => pm.User_Id == userId && pm.Plan_Id == planId);
        }

        public async Task<int> GetUserJoinedPlanCountAsync(int userId)
        {
            return await _context.PlanMembers.CountAsync(pm => pm.User_Id == userId);
        }
        public async Task AddUserToPlanAsync(int userId, int planId)
        {
            var bookPlan = await _context.BookPlan.FindAsync(planId);
            var role = bookPlan.User_Id == userId ? "組長" : "組員";
            var planMember = new PlanMembers
            {
                User_Id = userId,
                Plan_Id = planId,
                Role = role,
                JoinDate = DateTime.UtcNow
            };
            await _context.PlanMembers.AddAsync(planMember);
            await _context.SaveChangesAsync();
        }
        public async Task<string> GetUserRoleAsync(int userId, int planId)
        {
            return await _context.PlanMembers
                .Where(pm => pm.User_Id == userId && pm.Plan_Id == planId)
                .Select(pm => pm.Role)
                .FirstOrDefaultAsync();
        }

        public async Task RemoveUserFromPlanAsync(int userId, int planId)
        {
            var member = await _context.PlanMembers.FirstOrDefaultAsync(pm => pm.User_Id == userId && pm.Plan_Id == planId);
            if (member != null)
            {
                _context.PlanMembers.Remove(member);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> CopyPlanWithoutTopics(int planId, int userId)
        {
            var originalPlan = await _context.BookPlan.FirstOrDefaultAsync(p => p.Plan_Id == planId);
            if (originalPlan == null) return 0;

            var newPlan = new BookPlan
            {
                Plan_Name = originalPlan.Plan_Name + " (複製)",
                Plan_Goal = originalPlan.Plan_Goal,
                Plan_Type = originalPlan.Plan_Type,
                Plan_suject = originalPlan.Plan_suject,
                IsPublic = false,
                IsComplete = false,
                User_Id = userId
            };

            _context.BookPlan.Add(newPlan);
            // 統計複製次數 +1
            await _statisticService.AddCopyCountAsync(planId);
            await _context.SaveChangesAsync();
            return newPlan.Plan_Id;
        }
        public async Task<int> GetPlanLeaderIdAsync(int planId)
        {
            return await _context.BookPlan
                .Where(p => p.Plan_Id == planId)
                .Select(p => p.User_Id)  // 假設 Plan 中的 User_Id 是組長的 ID
                .FirstOrDefaultAsync();
        }
        public async Task RemoveProgressTrack(int UserId , int PlanId)
        {
            var result = await _context.ProgressTracking
                .Include(pt => pt.Learn)
                .ThenInclude(l => l.Plan)
                //這裡要再判斷是屬於哪個Plan
                .Where(pt => pt.User_Id == UserId && pt.Learn.Plan_Id == PlanId)
                .ToListAsync();
            if (result != null)
            {
                foreach (var delete in result)
                {
                    _context.Remove(delete);
                }
            }
            await _context.SaveChangesAsync();
        }
    }


}
