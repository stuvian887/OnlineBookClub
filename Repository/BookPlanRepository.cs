using Microsoft.EntityFrameworkCore;
using OnlineBookClub.Models;
using System;

namespace OnlineBookClub.Repository
{
    public class BookPlanRepository
    {
        private readonly OnlineBookClubContext _context;
        private readonly PlanMemberRepository _planMemberRepsitory;

        public BookPlanRepository(OnlineBookClubContext context, PlanMemberRepository planMemberRepsitory)
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

        public async Task<bool> Delete(int id)
        {
            var bookPlan = await _context.BookPlan.FindAsync(id);
            if (bookPlan == null)
                return false;

            _context.BookPlan.Remove(bookPlan);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
