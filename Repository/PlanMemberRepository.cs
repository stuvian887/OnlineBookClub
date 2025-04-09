using Microsoft.EntityFrameworkCore;
using OnlineBookClub.Models;

namespace OnlineBookClub.Repository
{
    public class PlanMemberRepository
    {
        private readonly OnlineBookClubContext _context;
        [ActivatorUtilitiesConstructor]
        public PlanMemberRepository(OnlineBookClubContext context)
        {
            _context = context;
        }

        public async Task<BookPlan> GetPlanByIdAsync(int planId)
        {
            return await _context.BookPlan.FirstOrDefaultAsync(p => p.Plan_Id == planId);
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
            await _context.SaveChangesAsync();
            return newPlan.Plan_Id;
        }
    }

}
