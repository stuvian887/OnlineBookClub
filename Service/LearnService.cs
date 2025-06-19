using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using OnlineBookClub.Repository;
using System.Runtime.CompilerServices;

namespace OnlineBookClub.Service
{
    public class LearnService
    {
        private readonly LearnRepository _learnRepository;
        public LearnService(LearnRepository learnRepository)
        {
            _learnRepository = learnRepository;
        }
        //public async Task<IEnumerable<LearnDTO>> GetAllLearn()
        //{
        //    return await _learnRepository.GetAllLearnAsync();

        //}
        public async Task<IEnumerable<LearnDTO>> GetLearn(int UserId , int PlanId)
        {
            return await _learnRepository.GetLearnByPlanIdAsync(UserId , PlanId);
        }
        public async Task<IEnumerable<LearnDTO>> GetLearnByChapter_Id(int UserId, int PlanId, int Chapter_Id)
        {
            return await _learnRepository.GetLearnByPlanIdAndChapterIdAsync(UserId, PlanId, Chapter_Id);
        }
        public async Task<(IEnumerable<CalendarLearnDTO> , string Message)> GetLearnByCalendar(int UserId , DateTime? BeginTime , DateTime? EndTime)
        {
            return await _learnRepository.GetLearnByCalendar(UserId , BeginTime , EndTime);
        }
        public async Task<(LearnDTO, string Message)> CreateLearn( int UserId , int PlanId , int Chapter_Id, LearnDTO newData)
        {
            return await _learnRepository.CreateLearnAsync(UserId, PlanId, Chapter_Id, newData);
        }
        public async Task<(LearnDTO , string Message)> UpdateLearn(int UserId , int PlanId , int Chapter_Id, int Learn_Index ,  LearnDTO UpdateData)
        {
            return await _learnRepository.UpdateLearnAsync(UserId , PlanId, Chapter_Id, Learn_Index, UpdateData);
        }
        public async Task<(LearnDTO , string Message)> DeleteLearn(int UserId , int PlanId ,int Chapter_Id , int Learn_Index)
        {
            return await _learnRepository.DeleteLearnAsync(UserId , PlanId ,Chapter_Id,  Learn_Index);
        }
        public async Task<IEnumerable<MemberProgressDTO>> GetMemberProgressAsync(int UserId , int PlanId)
        {
            return await _learnRepository.GetMemberPassLearnPersentAsync(UserId , PlanId);
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
        public async Task<(LearnDTO, string Message)> copy(int UserId, int PlanId,  LearnDTO UpdateData)
        {
            return await _learnRepository.copylearnAsync(UserId, PlanId, UpdateData);
        }
        public async Task<IEnumerable<PassLearnDTO>> GetMemberByLearn(int leaderId , int planId , int Learn_Index)
        {
            return await _learnRepository.GetMemberByPlansAsync(leaderId, planId, Learn_Index);
        }
        public void MoveLearnToChapter()
        {
            _learnRepository.MoveLearnToChapter();
        }
    }
}
