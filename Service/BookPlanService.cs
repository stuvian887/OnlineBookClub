using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using OnlineBookClub.Repository;

namespace OnlineBookClub.Service
{
    public class BookPlanService
    {
        private readonly BookPlanRepository _repository;

        public BookPlanService(BookPlanRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<BookPlan>> GetAllPublicPlans()
        {
            return await _repository.GetAllPublicPlans();
        }

        public async Task<BookPlan> GetById(int id)
        {
            return await _repository.GetById(id);
        }

        public async Task<BookPlan> Create(BookPlanDTO bookPlanDto)
        {
            var bookPlan = new BookPlan
            {
                Plan_Name = bookPlanDto.PlanName,
                Plan_Goal = bookPlanDto.PlanGoal,
                Plan_Type = bookPlanDto.PlanType,
                Plan_suject = bookPlanDto.PlanSubject,
                IsPublic = bookPlanDto.IsPublic,
                IsComplete = bookPlanDto.IsComplete,
                User_Id = bookPlanDto.UserId
            };

            return await _repository.Create(bookPlan);
        }

        public async Task<BookPlan> Update(int id, BookPlanDTO bookPlanDto)
        {
            var bookPlan = await _repository.GetById(id);
            if (bookPlan == null) return null;

            bookPlan.Plan_Name = bookPlanDto.PlanName;
            bookPlan.Plan_Goal = bookPlanDto.PlanGoal;
            bookPlan.Plan_Type = bookPlanDto.PlanType;
            bookPlan.Plan_suject = bookPlanDto.PlanSubject;
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
