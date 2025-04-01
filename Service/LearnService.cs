using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using OnlineBookClub.Repository;

namespace OnlineBookClub.Service
{
    public class LearnService
    {
        private readonly LearnRepository _learnRepository;
        public LearnService(LearnRepository learnRepository)
        {
            _learnRepository = learnRepository;
        }
        public async Task<IEnumerable<LearnDTO>> GetAllLearn()
        {
            return await _learnRepository.GetAllLearn();

        }
        public async Task<IEnumerable<LearnDTO>> GetLearn(int PlanId)
        {
            return await _learnRepository.GetLearnByPlanId(PlanId);
        }
        public async Task<LearnDTO> CreateLearn(int PlanId , LearnDTO newData)
        {
            return await _learnRepository.CreateLearn(PlanId, newData);
        }
        public async Task<LearnDTO> UpdateLearn(int PlanId , int LearnId , LearnDTO UpdateData)
        {
            return await _learnRepository.UpdateLearn(PlanId, LearnId, UpdateData);
        }
        public async Task<LearnDTO> DeleteLearn(int PlanId , int LearnId)
        {
            return await _learnRepository.DeleteLearn(PlanId , LearnId);
        }
    }
}
