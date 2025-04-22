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
            return await _learnRepository.GetAllLearnAsync();

        }
        public async Task<IEnumerable<LearnDTO>> GetLearn(int PlanId)
        {
            return await _learnRepository.GetLearnByPlanIdAsync(PlanId);
        }
        public async Task<(LearnDTO, string Message)> CreateLearn(int PlanId , LearnDTO newData)
        {
            return await _learnRepository.CreateLearnAsync(PlanId, newData);
        }
        public async Task<(LearnDTO , string Message)> UpdateLearn(int UserId , int PlanId , int Learn_Index , LearnDTO UpdateData)
        {
            return await _learnRepository.UpdateLearnAsync(UserId , PlanId, Learn_Index, UpdateData);
        }
        public async Task<(LearnDTO , string Message)> DeleteLearn(int UserId , int PlanId , int Learn_Index)
        {
            return await _learnRepository.DeleteLearnAsync(UserId , PlanId , Learn_Index);
        }
        public async Task<IEnumerable<Answer_RecordDTO>> GetAnswer_Record(int UserId , int PlanId , int Learn_Index)
        {
            return await _learnRepository.GetRecordAsync(UserId, PlanId, Learn_Index);
        }
        public async Task<IEnumerable<Answer_RecordDTO>> CreateAnswer_Record(int UserId , AnswerSubmissionDTO Answer)
        {
            return await _learnRepository.CreateRecordAsync(UserId , Answer);
        }
        public async Task<ProgressTrackingDTO> PassProgressAsync (int UserId , int PlanId , int Learn_Index)
        {
            return await _learnRepository.PassProgressAsync(UserId, PlanId, Learn_Index);
        }
    }
}
