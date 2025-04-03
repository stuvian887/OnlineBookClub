using Microsoft.AspNetCore.Http;
using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using OnlineBookClub.Repository;
using OnlineBookClub.Token;
using System.Security.Claims;

namespace OnlineBookClub.Service
{
    public class BookPlanService
    {
        
        private readonly BookPlanRepository _repository;
        private readonly JwtService _jwtService;
        private readonly PlanMemberRepository _memberRepository;
        public BookPlanService(BookPlanRepository repository, JwtService jwtService, PlanMemberRepository memberRepository)
        {
            _jwtService = jwtService;
            _repository = repository;
            _memberRepository = memberRepository;
        }

        public async Task<IEnumerable<BookPlan>> GetAllPublicPlans()
        {
            return await _repository.GetAllPublicPlans();
        }

        public async Task<BookPlan> GetById(int id)
        {
            return await _repository.GetById(id);
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
    }
}
