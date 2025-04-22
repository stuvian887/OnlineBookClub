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

        public async Task<List<BookPlanDTO>> GetPublicPlansBySearchAsync(string keyword, ForPaging paging)
        {
            var query = _context.BookPlan.Where(p => p.IsPublic);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p =>
                    p.Plan_Name.Contains(keyword) ||
                    p.Plan_Goal.Contains(keyword));
            }

            // 計算總筆數，設定 MaxPage
            int totalCount = await query.CountAsync();
            paging.MaxPage = (int)Math.Ceiling((double)totalCount / paging.ItemNum);
            paging.SetRightPage();

            // 查詢目前頁的資料
            var pagedPlans = await query
                .OrderByDescending(p => p.Plan_Id)
                .Skip((paging.NowPage - 1) * paging.ItemNum)
                .Take(paging.ItemNum)
                .ToListAsync();

            // 查出創建者名稱
            var creatorIds = pagedPlans.Select(p => p.User_Id).Distinct().ToList();
            var userMap = await _context.Members
                .Where(u => creatorIds.Contains(u.User_Id))
                .ToDictionaryAsync(u => u.User_Id, u => u.UserName);

            // 將資料轉成 DTO
            var dtoList = pagedPlans.Select(p => new BookPlanDTO
            {
                Plan_ID = p.Plan_Id,
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
                Plan_ID = p.Plan_Id,
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
