using Azure.Core;
using Microsoft.EntityFrameworkCore.Query.Internal;
using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using System.Reflection.Metadata.Ecma335;

namespace OnlineBookClub.Repository
{
    public class TopicRepository
    {
        private readonly OnlineBookClubContext _context;
        public TopicRepository(OnlineBookClubContext context)
        {
            _context = context;
        }
        public async Task<TopicDTO> GetTopicAsync(int PlanId, int LearnId)
        {
            var Plan = await _context.BookPlan.FindAsync(PlanId);
            if (Plan != null)
            {
                var Learn = await _context.Learn.FindAsync(LearnId);
                if (Learn != null)
                {
                    var result = (from a in _context.Topic
                                  select new TopicDTO
                                  {
                                      Question_Id = a.Question_Id,
                                      Question = a.Question,
                                      Option_A = a.Option_A,
                                      Option_B = a.Option_B,
                                      Option_C = a.Option_C,
                                      Option_D = a.Option_D,
                                      Answer = a.Answer,
                                  });
                    return (TopicDTO)result;
                }
            }
            return null;
        }
        public async Task<TopicDTO> CreateTopicAsync(int PlanId, int LearnId, TopicDTO InsertTopic)
        {
            var Plan = await _context.BookPlan.FindAsync(PlanId);
            if (Plan != null)
            {
                var Learn = await _context.Learn.FindAsync(LearnId);
                if (Learn != null)
                {
                    Topic topic = new Topic
                    {
                        Learn_Id = LearnId,
                        Question_Id = InsertTopic.Question_Id,
                        Question = InsertTopic.Question,
                        Option_A = InsertTopic.Option_A,
                        Option_B = InsertTopic.Option_B,
                        Option_C = InsertTopic.Option_C,
                        Option_D = InsertTopic.Option_D,
                        Answer = InsertTopic.Answer
                    };
                    await _context.Topic.AddAsync(topic);
                    await _context.SaveChangesAsync();
                    TopicDTO resultDTO = new TopicDTO
                    {
                        Question_Id = topic.Question_Id
                    };
                    return resultDTO;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        public async Task<TopicDTO> UpdateTopic(int PlanId, int LearnId, int TopicId, int QuestionId, TopicDTO EditTopic)
        {
            var Plan = await _context.BookPlan.FindAsync(PlanId);
            if (Plan != null)
            {
                var Learn = await _context.Learn.FindAsync(LearnId);
                if (Learn != null)
                {
                    Topic topic = await _context.Topic.FindAsync(TopicId, QuestionId);
                    if (topic != null)
                    {
                        topic.Question_Id = QuestionId;
                        topic.Question = EditTopic.Question;
                        topic.Option_A = EditTopic.Option_A;
                        topic.Option_B = EditTopic.Option_B;
                        topic.Option_C = EditTopic.Option_C;
                        topic.Option_D = EditTopic.Option_D;
                        topic.Answer = EditTopic.Answer;
                        _context.Topic.Update(topic);
                        await _context.SaveChangesAsync();
                        TopicDTO resultDTO = new TopicDTO
                        {
                            Question = topic.Question,
                        };
                        return resultDTO;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        public async Task<TopicDTO> DeleteTopic(int PlanId, int LearnId, int TopicId, int QuestionId)
        {
            var Plan = await _context.BookPlan.FindAsync(PlanId);
            if (Plan != null)
            {
                var Learn = await _context.Learn.FindAsync(LearnId);
                if (Learn != null)
                {
                    Topic topic = await _context.Topic.FindAsync(TopicId, QuestionId);
                    if (topic != null)
                    {
                        _context.Topic.Remove(topic);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            return null;
        }
        
    }
}

