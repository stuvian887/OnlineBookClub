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
        public async Task<IEnumerable<TopicDTO>> GetTopicAsync(int PlanId, int Learn_Index)
        {
            return await _topicRepository.GetTopicAsync(PlanId, Learn_Index);
        }
        public async Task<(TopicDTO, string Message)> CreateTopic(int UserId , int PlanId, int Learn_Index, TopicDTO InsertTopic)
        {
            return await _topicRepository.CreateTopicAsync(UserId , PlanId, Learn_Index, InsertTopic);
        }
        public async Task<(TopicDTO,string Message)> UpdateTopic(int UserId , int PlanId, int Learn_Index, int QuestionId, TopicDTO EditTopic)
        {
            return await _topicRepository.UpdateTopicAsync(UserId , PlanId, Learn_Index,QuestionId, EditTopic);
        }
        public async Task<(TopicDTO,string Message)> DeleteTopic(int UserId , int PlanId , int Learn_Index, int QuestionId)
        {
            return await _topicRepository.DeleteTopicAsync(UserId , PlanId, Learn_Index, QuestionId);
        }
    }
}
