using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;

namespace OnlineBookClub.Repository
{
    public class TopicRepository
    {
        private readonly OnlineBookClubContext _context;
        private readonly PlanMemberRepository _memberRepository;
        public TopicRepository(OnlineBookClubContext context, PlanMemberRepository memberRepository)
        {
            _context = context;
            _memberRepository = memberRepository;
        }
        public async Task<IEnumerable<TopicDTO>> GetTopicAsync(int PlanId, int Learn_Index)
        {
            var plan = await _context.BookPlan.FindAsync(PlanId);
            if (plan == null)
            {
                return null;
            }
            var learn = await _context.Learn.Where(l => l.Plan_Id == PlanId).FirstOrDefaultAsync(l => l.Learn_Index == Learn_Index);
            if (learn == null)
            {
                return null;
            }
            var topics = await _context.Topic
                .Where(t => t.Learn_Id == learn.Learn_Id)
                .ToListAsync();
            var result = topics.Select(t => new TopicDTO
            {
                Learn_Id = learn.Learn_Id,
                Question_Id = t.Question_Id,
                Question = t.Question,
                Option_A = t.Option_A,
                Option_B = t.Option_B,
                Option_C = t.Option_C,
                Option_D = t.Option_D,
                Answer = t.Answer
            });
            return result;
        }
        public async Task<(TopicDTO, string Message)> CreateTopicAsync(int UserId, int PlanId, int Learn_Index, TopicDTO InsertTopic)
        {
            var role = await _memberRepository.GetUserRoleAsync(UserId, PlanId);
            if (role == "組長")
            {
                var plan = await _context.BookPlan.FindAsync(PlanId);
                if (plan == null)
                {
                    return (null, "找不到該計畫");
                }
                var learn = await _context.Learn.Where(l => l.Plan_Id == PlanId).FirstOrDefaultAsync(l => l.Learn_Index == Learn_Index);
                if (learn == null)
                {
                    return (null, "找不到該學習內容");
                }   
                var existingTopic = await _context.Topic.FirstOrDefaultAsync(t => t.Learn_Id == learn.Learn_Id && t.Question_Id == InsertTopic.Question_Id);
                if (existingTopic != null)
                {
                    return (null, "該題號已存在");
                }
                Topic topic = new Topic
                {
                    Learn_Id = learn.Learn_Id,
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
                    Question_Id = topic.Question_Id,
                    Learn_Id = topic.Learn_Id,
                    Question = topic.Question,
                    Option_A = topic.Option_A,
                    Option_B = topic.Option_B,
                    Option_C = topic.Option_C,
                    Option_D = topic.Option_D,
                    Answer = topic.Answer
                };
                return (resultDTO, "題目新增成功");
            }
            else if (role == "組員")
            {
                return (null, "你沒有權限這麼做");
            }
            else
            {
                return (null, "找不到你是誰");
            }
        }
        public async Task<(TopicDTO, string Message)> UpdateTopicAsync(int UserId, int PlanId, int Learn_Index, int QuestionId, TopicDTO EditTopic)
        {
            var role = await _memberRepository.GetUserRoleAsync(UserId, PlanId);
            if (role == "組長")
            {
                var plan = await _context.BookPlan.FindAsync(PlanId);
                if (plan == null)
                {
                    return (null, "找不到該計畫");
                }
                var learn = await _context.Learn.Where(l => l.Plan_Id == PlanId).FirstOrDefaultAsync(l => l.Learn_Index == Learn_Index);
                if (learn == null)
                {
                    return (null, "找不到該學習內容");
                }
                var topic = await _context.Topic.FirstOrDefaultAsync(t => t.Learn_Id == learn.Learn_Id && t.Question_Id == EditTopic.Question_Id);
                if (topic != null)
                {
                    topic.Learn_Id = learn.Learn_Id;
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
                        Question_Id = topic.Question_Id,
                        Learn_Id = topic.Learn_Id,
                        Question = topic.Question,
                        Option_A = topic.Option_A,
                        Option_B = topic.Option_B,
                        Option_C = topic.Option_C,
                        Option_D = topic.Option_D,
                        Answer = topic.Answer
                    };
                    return (resultDTO, "題目修改成功");
                }
                return (null, "錯誤，該題號不存在");
            }
            else if (role == "組員")
            {
                return (null, "你沒有權限這麼做");
            }
            else
            {
                return (null, "找不到你是誰");
            }
        }
        public async Task<(TopicDTO,string Message)> DeleteTopicAsync(int UserId , int PlanId, int Learn_Index, int QuestionId)
        {
            var Role = await _memberRepository.GetUserRoleAsync(UserId, PlanId);
            if (Role == "組長")
            {
                var plan = await _context.BookPlan.FindAsync(PlanId);
                if (plan == null)
                {
                    return (null, "找不到該計畫");
                }

                var learn = await _context.Learn.Where(l => l.Plan_Id == PlanId).FirstOrDefaultAsync(l => l.Learn_Index == Learn_Index);
                if (learn == null)
                {
                    return (null, "找不到該學習內容");
                }

                var topic = await _context.Topic
                    .FirstOrDefaultAsync(t => t.Learn_Id == learn.Learn_Id && t.Question_Id == QuestionId);

                if (topic == null)
                {
                    return (null, "找不到該題號");
                }

                _context.Topic.Remove(topic);
                await _context.SaveChangesAsync();
                TopicDTO resultDTO = new TopicDTO
                {
                    Question = topic.Question
                };
                return (resultDTO, "刪除成功");
            }
            else if (Role == "組員")
            {
                return (null, "你沒有權限這麼做");
            }
            else
            {
                return (null, "找不到你是誰");
            }
        }
        
    }
}

