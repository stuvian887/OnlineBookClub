using OnlineBookClub.DTO;
using OnlineBookClub.Repository;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata.Ecma335;

namespace OnlineBookClub.Service
{
    public class TopicService
    {
        private readonly TopicRepository _topicRepository;
        public TopicService(TopicRepository topicRepository)
        {
            _topicRepository = topicRepository;
        }
        public async Task<IEnumerable<TopicDTO>> GetTopicAsync(int PlanId, int LearnId)
        {
            return await _topicRepository.GetTopicAsync(PlanId, LearnId);
        }
        public async Task<(TopicDTO, string Message)> CreateTopic(int UserId , int PlanId, int LearnId, TopicDTO InsertTopic)
        {
            return await _topicRepository.CreateTopicAsync(UserId , PlanId, LearnId, InsertTopic);
        }
        public async Task<(TopicDTO,string Message)> UpdateTopic(int UserId , int PlanId, int LearnId, int QuestionId, TopicDTO EditTopic)
        {
            return await _topicRepository.UpdateTopicAsync(UserId , PlanId, LearnId,QuestionId, EditTopic);
        }
        public async Task<(TopicDTO,string Message)> DeleteTopic(int UserId , int PlanId , int LearnId, int QuestionId)
        {
            return await _topicRepository.DeleteTopicAsync(UserId , PlanId, LearnId, QuestionId);
        }
    }
}
