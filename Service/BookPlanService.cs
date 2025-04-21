using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using OnlineBookClub.Repository;
using OnlineBookClub.Token;
using System.Numerics;
using System.Security.Claims;

namespace OnlineBookClub.Service
{
    public class BookPlanService
    {
        
        private readonly BookPlanRepository _repository;
        private readonly JwtService _jwtService;
        private readonly PlanMemberRepository _memberRepository;
        private readonly StatisticService _statisticService;
        public BookPlanService(BookPlanRepository repository, JwtService jwtService, PlanMemberRepository memberRepository, StatisticService statisticService)
        {
            _jwtService = jwtService;
            _repository = repository;
            _memberRepository = memberRepository;
            _statisticService = statisticService;
        }

        public async Task<IEnumerable<BookPlanDTO>> GetAllPublicPlans()
        {
            return await _repository.GetAllPublicPlansWithCreatorName();
        }

        public async Task<BookPlan> GetById(int id)
        {
            await _statisticService.AddViewTimesAsync(id);

            return await _repository.GetById(id);
        }

        public async Task<List<BookPlanDTO>> GetuserById(int userid)
        {
            

            return await _repository.GetPlansWithCreatorNameByUserId(userid);
        }

        public async Task<BookPlan> Create(BookPlanDTO bookPlanDto,int id)
        {
           
            var bookPlan = new BookPlan
            {
                Plan_Name = bookPlanDto.Plan_Name,
                Plan_Goal = bookPlanDto.Plan_Goal,
                Plan_Type = bookPlanDto.Plan_Type,
                Plan_suject = bookPlanDto.Plan_Suject,
                IsPublic = bookPlanDto.IsPublic,
                IsComplete = bookPlanDto.IsComplete,
                User_Id = id
            };
            return await _repository.Create(bookPlan);
        }

        public async Task<BookPlan> Update(int id, BookPlanDTO bookPlanDto)
        {
            var bookPlan = await _repository.GetById(id);
            if (bookPlan == null) return null;

            bookPlan.Plan_Name = bookPlanDto.Plan_Name;
            bookPlan.Plan_Goal = bookPlanDto.Plan_Goal;
            bookPlan.Plan_Type = bookPlanDto.Plan_Type;
            bookPlan.Plan_suject = bookPlanDto.Plan_Suject;
            bookPlan.IsPublic = bookPlanDto.IsPublic;
            bookPlan.IsComplete = bookPlanDto.IsComplete;

            return await _repository.Update(bookPlan);
        }

        public async Task<bool> Delete(int id)
        {
            return await _repository.Delete(id);
        }
       
            public async Task<bool> CopyPlanAsync(int planId, int userId)
        {
            var originalPlan = await _repository.GetById(planId);
            if (originalPlan == null)
                return false;

            // 建立新計畫（複製基本資訊）
            var newPlan = new BookPlan
            {
                Plan_Name = originalPlan.Plan_Name,
                Plan_Goal = originalPlan.Plan_Goal,
                Plan_suject = originalPlan.Plan_suject,
                Plan_Type= originalPlan.Plan_Type,
                IsPublic = false, // 預設複製後為私人
                User_Id = userId,
                
            };

            // 儲存新計畫
            await _repository.Create(newPlan);

            

            return true;


        }
    }
}
