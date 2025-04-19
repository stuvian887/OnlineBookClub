using Microsoft.EntityFrameworkCore;
using OnlineBookClub.Models;
using System;
using System.Linq;

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

        public async Task<IEnumerable<BookPlan>> GetAllPublicPlans()
        {
            return await _context.BookPlan.Where(p => p.IsPublic).ToListAsync();
        }

        public async Task<BookPlan> GetById(int id)
        {
            return await _context.BookPlan.FindAsync(id);
        }

        public async Task<List<BookPlan>> GetByuserId(int userid)
        {
            var createdPlans = await _context.BookPlan
                 .Where(p => p.User_Id == userid)
                 .ToListAsync();

            // 參加的計畫（取得對應的 BookPlan）
            var joinedPlanIds = await _context.PlanMembers
                .Where(m => m.User_Id == userid)
                .Select(m => m.Plan_Id)
                .ToListAsync();

            var joinedPlans = await _context.BookPlan
                .Where(p => joinedPlanIds.Contains(p.Plan_Id))
                .ToListAsync();

            // 合併並去除重複（雖然理論上不會重複）
            var allPlans = createdPlans
                .Concat(joinedPlans)
                .Distinct()
                .ToList();

            return allPlans;
        }

        public async Task<BookPlan> Create(BookPlan bookPlan)
        {

            _context.BookPlan.Add(bookPlan);
            await _context.SaveChangesAsync();
            await _planMemberRepsitory.AddUserToPlanAsync(bookPlan.User_Id,bookPlan.Plan_Id);
            return bookPlan;
        }

        public async Task<BookPlan> Update(BookPlan bookPlan)
        {
            _context.BookPlan.Update(bookPlan);
            await _context.SaveChangesAsync();
            return bookPlan;
        }

        public async Task<bool> Delete(int PlanId)
        {
            var bookPlan = await _context.BookPlan.FindAsync(PlanId);
            if (bookPlan == null)
                return false;

            _context.BookPlan.Remove(bookPlan);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
