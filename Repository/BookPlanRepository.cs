using Microsoft.EntityFrameworkCore;
using OnlineBookClub.DTO;
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

        public async Task<IEnumerable<BookPlanDTO>> GetAllPublicPlansWithCreatorName()
        {
            // 1. 取得所有公開的 BookPlan
            var publicPlans = await _context.BookPlan
                .Where(p => p.IsPublic)
                .ToListAsync();

            // 2. 找出所有創建者的 User_Id
            var creatorIds = publicPlans.Select(p => p.User_Id).Distinct().ToList();

            // 3. 從 User 資料表取得對應的 User_Name（記得確認你的 User 表叫什麼）
            var userMap = await _context.Members
                .Where(u => creatorIds.Contains(u.User_Id))
                .ToDictionaryAsync(u => u.User_Id, u => u.UserName);

            // 4. 組成 DTO
            var dtoList = publicPlans.Select(p => new BookPlanDTO
            {
                Plan_ID=p.Plan_Id,
                Plan_Name = p.Plan_Name,
                Plan_Goal = p.Plan_Goal,
                Plan_Type = p.Plan_Type,
                Plan_Suject = p.Plan_suject,
                IsPublic = p.IsPublic,
                IsComplete = p.IsComplete,
                CreatorName = userMap.ContainsKey(p.User_Id) ? userMap[p.User_Id] : "未知使用者"
            }).ToList();

            return dtoList;
        }


        public async Task<BookPlan> GetById(int id)
        {
            return await _context.BookPlan.FindAsync(id);
        }

        public async Task<List<BookPlanDTO>> GetPlansWithCreatorNameByUserId(int userId)
        {
            var createdPlans = await _context.BookPlan
                .Where(p => p.User_Id == userId)
                .ToListAsync();

            var joinedPlanIds = await _context.PlanMembers
                .Where(m => m.User_Id == userId)
                .Select(m => m.Plan_Id)
                .ToListAsync();

            var joinedPlans = await _context.BookPlan
                .Where(p => joinedPlanIds.Contains(p.Plan_Id))
                .ToListAsync();

            var allPlans = createdPlans.Concat(joinedPlans).Distinct().ToList();

            //  取得所有相關的創建者 ID
            var creatorIds = allPlans.Select(p => p.User_Id).Distinct().ToList();

            //  查出所有創建者的名字
            var users = await _context.Members
                .Where(u => creatorIds.Contains(u.User_Id))
                .ToDictionaryAsync(u => u.User_Id, u => u.UserName);

            //組成 DTO
            var result = allPlans.Select(p => new BookPlanDTO
            {
                Plan_ID=p.Plan_Id,
                Plan_Name = p.Plan_Name,
                Plan_Goal = p.Plan_Goal,
                Plan_Type = p.Plan_Type,
                Plan_Suject = p.Plan_suject,
                IsPublic = p.IsPublic,
                IsComplete = p.IsComplete,
                CreatorName = users.ContainsKey(p.User_Id) ? users[p.User_Id] : "未知使用者"
            
            }).ToList();

            return result;
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
