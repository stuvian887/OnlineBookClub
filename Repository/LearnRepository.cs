using Microsoft.EntityFrameworkCore;
using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using System.Security.Claims;

namespace OnlineBookClub.Repository
{
    public class LearnRepository
    {

        private readonly OnlineBookClubContext _context;
        private readonly PlanMemberRepository _planMemberRepository;
        public LearnRepository(OnlineBookClubContext context, PlanMemberRepository planMemberRepository)
        {
            _context = context;
            _planMemberRepository = planMemberRepository;
        }
        public async Task<IEnumerable<LearnDTO>> GetAllLearnAsync()
        {
            var result = (from a in _context.Learn
                          select new LearnDTO
                          {
                              Plan_Id = a.Plan_Id,
                              Learn_Index = a.Learn_Index,
                              Learn_Name = a.Learn_Name,
                              Pass_Standard = a.Pass_Standard,
                              DueTime = a.DueTime,
                              Manual_Check = a.Manual_Check,
                              ProgressTracking = a.ProgressTracking
                          });
            var list = await result.ToListAsync();
            return list.Select(a => GetProgressTrack(a));
        }
        public async Task<IEnumerable<LearnDTO>> GetLearnByPlanIdAsync(int PlanId)
        {
            var result = (from a in _context.Learn
                          where PlanId == a.Plan_Id
                          select new LearnDTO
                          {
                              Plan_Id = a.Plan_Id,
                              Learn_Index = a.Learn_Index,
                              Learn_Name = a.Learn_Name,
                              Pass_Standard = a.Pass_Standard,
                              DueTime = a.DueTime,
                              Manual_Check = a.Manual_Check,
                              ProgressTracking = a.ProgressTracking
                          });
            var list = await result.ToListAsync();
            return list.Select(a => GetProgressTrack(a));
        }
        public async Task<Learn> GetLearnByLearnIdAsync(int LearnId)
        {
            var result = await _context.Learn.SingleOrDefaultAsync(l => l.Learn_Id == LearnId);
            return result;
        }
        public async Task<BookPlan> CheckLearnByPlanIdAsync(int PlanId)
        {
            return await _context.BookPlan.Include(bp => bp.Learn).SingleOrDefaultAsync(p => p.Plan_Id == PlanId);
        }

        public async Task<LearnDTO> CreateLearnAsync(int PlanId, LearnDTO InsertData)
        {
            BookPlan FindPlan = await CheckLearnByPlanIdAsync(PlanId);
            if (FindPlan != null)
            {
                Learn learn = new Learn();
                learn.Plan_Id = PlanId;
                learn.Learn_Name = InsertData.Learn_Name;
                learn.Learn_Index = InsertData.Learn_Index;
                learn.Pass_Standard = InsertData.Pass_Standard;
                learn.DueTime = InsertData.DueTime;
                _context.Learn.Add(learn);
                await _context.SaveChangesAsync();
                LearnDTO resultDTO = new LearnDTO()
                {
                    Learn_Name = learn.Learn_Name,
                };
                await CreateProgressTrackAsync(PlanId, learn.Learn_Id);
                return resultDTO;
            }
            else
            {
                return null;
            }
        }
        public async Task<(LearnDTO, string Message)> UpdateLearnAsync(int UserId, int PlanId, int LearnId, LearnDTO UpdateData)
        {
            var Role = await _planMemberRepository.GetUserRoleAsync(UserId, PlanId);
            if (Role == "組長")
            {
                BookPlan UpdateDataPlan = await CheckLearnByPlanIdAsync(PlanId);
                if (UpdateDataPlan != null)
                {
                    Learn UpdateLearn = await GetLearnByLearnIdAsync(LearnId);
                    if (UpdateLearn != null && UpdateLearn.Plan_Id == PlanId)
                    {
                        UpdateLearn.Plan_Id = UpdateDataPlan.Plan_Id;
                        UpdateLearn.Learn_Name = UpdateData.Learn_Name;
                        UpdateLearn.Learn_Index = UpdateData.Learn_Index;
                        UpdateLearn.Pass_Standard = UpdateLearn.Pass_Standard;
                        UpdateLearn.DueTime = UpdateData.DueTime;
                        UpdateLearn.Manual_Check = UpdateData.Manual_Check;
                        _context.Update(UpdateLearn);
                        await _context.SaveChangesAsync();
                        LearnDTO resultDTO = new LearnDTO();
                        resultDTO.Learn_Name = UpdateLearn.Learn_Name;
                        return (resultDTO, "修改計畫成功");
                    }
                    else
                    {
                        return (null, "錯誤，找不到該學習內容");
                    }
                }
                else
                {
                    return (null, "錯誤，找不到該計畫");
                }
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

        public async Task<(LearnDTO, string Message)> DeleteLearnAsync(int UserId, int PlanId, int LearnId)
        {
            var Role = await _planMemberRepository.GetUserRoleAsync(UserId, PlanId);
            if (Role == "組長")
            {
                BookPlan DeleteLearnOfPlan = await CheckLearnByPlanIdAsync(PlanId);
                if (DeleteLearnOfPlan != null)
                {
                    Learn DeleteLearn = await GetLearnByLearnIdAsync(LearnId);
                    if (DeleteLearn != null && DeleteLearn.Plan_Id == PlanId)
                    {
                        _context.Remove(DeleteLearn);
                        await _context.SaveChangesAsync();
                        LearnDTO resultDTO = new LearnDTO();
                        resultDTO.Learn_Name = DeleteLearn.Learn_Name;
                        return (resultDTO, "刪除成功");
                    }
                    else
                    {
                        return (null, "找不到此學習");
                    }
                }
                else
                {
                    return (null, "找不到此計畫");
                }
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
        public async Task<LearnDTO> DeleteAllLearnAsync(int PlanId)
        {
            BookPlan DeleteLearnOfPlan = await CheckLearnByPlanIdAsync(PlanId);
            if (DeleteLearnOfPlan != null)
            {
                var learns = await _context.Learn.Where(a => a.Plan_Id == PlanId).ToListAsync();
                foreach (var item in learns)
                {
                    if (item != null && item.Plan_Id == PlanId)
                    {
                        _context.Learn.Remove(item);
                    }
                }
                await _context.SaveChangesAsync();
                return null;
            }
            else
            {
                return null;
            }
        }
        public static LearnDTO GetProgressTrack(LearnDTO b)
        {
            List<ProgressTrackingDTO> Progress = new List<ProgressTrackingDTO>();
            if (b.ProgressTracking != null)
            {
                foreach (var temp in b.ProgressTracking)
                {
                    if (temp != null)
                    {
                        ProgressTrackingDTO progress = new ProgressTrackingDTO
                        {
                            Progress_Id = temp.Progress_Id,
                            User_Id = temp.User_Id,
                            Learn_Id = temp.Learn_Id,
                            Status = temp.Status,
                            CompletionDate = temp.CompletionDate,
                        };
                        Progress.Add(progress);
                    }
                }
            }
            return b;
        }
        public async Task<IEnumerable<ProgressTrackingDTO>> CreateAllProgressTrackAsync(int UserId, int PlanId)
        {
            var Learns = await _context.Learn.Where(l => l.Plan_Id == PlanId).ToListAsync();
            if (Learns != null)
            {
                List<ProgressTrackingDTO> resultDTOs = new List<ProgressTrackingDTO>();
                foreach (var learn in Learns)
                {
                    ProgressTracking progress = new ProgressTracking
                    {
                        User_Id = UserId,
                        Learn_Id = learn.Learn_Id,
                        Status = false,
                    };
                    await _context.ProgressTracking.AddAsync(progress);

                    ProgressTrackingDTO dto = new ProgressTrackingDTO
                    {
                        User_Id = progress.User_Id,
                        Learn_Id = progress.Learn_Id,
                        Status = progress.Status,
                    };
                    resultDTOs.Add(dto);
                }
                await _context.SaveChangesAsync();
                return resultDTOs;
            }
            return null;
        }
        //還沒Debug
        public async Task<IEnumerable<ProgressTrackingDTO>> CreateProgressTrackAsync(int PlanId, int LearnId)
        {
            var Members = await _context.PlanMembers.Where(pm => pm.Plan_Id == PlanId).ToListAsync();
            List<ProgressTrackingDTO> resultDTO = new List<ProgressTrackingDTO>();

            if (Members != null)
            {
                foreach (var member in Members)
                {
                    ProgressTracking progress = new ProgressTracking
                    {
                        User_Id = member.User_Id,
                        Learn_Id = LearnId,
                        Status = false,
                    };
                    await _context.ProgressTracking.AddAsync(progress);

                    ProgressTrackingDTO dto = new ProgressTrackingDTO
                    {
                        User_Id = progress.User_Id,
                        Learn_Id = progress.Learn_Id,
                        Status = progress.Status,
                    };
                    resultDTO.Add(dto);
                }
                await _context.SaveChangesAsync();
                return resultDTO;
            }
            return null;
        }
        //還沒Debug
        public async Task<IEnumerable<Answer_RecordDTO>> CreateRecordAsync(int UserId, int PlanId, int LearnId)
        {
            List<Answer_RecordDTO> resultDTO = new List<Answer_RecordDTO>();
            var User = await _planMemberRepository.GetUserRoleAsync(UserId, PlanId);
            if (User != null)
            {
                var Plan = await _context.BookPlan.FindAsync(PlanId);
                if (Plan != null)
                {
                    var Learn = await _context.Learn.FindAsync(LearnId);
                    if (Learn != null & Learn.Plan_Id == Plan.Plan_Id)
                    {
                        //缺少判斷User 如果有相同人作答 可能會蓋掉其他人資料?
                        var Topic = await _context.Topic.Where(t => t.Learn_Id == LearnId).ToListAsync();
                        foreach (var topics in Topic)
                        {
                            
                            Answer_Record answer_Record = new Answer_Record();
                            answer_Record.User_Id = UserId;
                            answer_Record.Learn_Id = Learn.Learn_Id;
                            answer_Record.Topic_Id = topics.Topic_Id;
                            answer_Record.AnswerDate = DateTime.Now;
                            answer_Record.Answer = topics.Answer;
                            answer_Record.times = answer_Record.times + 1;
                            await _context.Answer_Record.AddAsync(answer_Record);

                            Answer_RecordDTO result = new Answer_RecordDTO
                            {
                                User_Id = answer_Record.User_Id,
                                Learn_Id = answer_Record.Learn_Id,
                                Topic_Id = topics.Topic_Id,
                                AnswerDate = answer_Record.AnswerDate,
                                Answer = answer_Record.Answer,
                                times = answer_Record.times
                            };
                            resultDTO.Add(result);
                        }
                        await _context.SaveChangesAsync();
                        return resultDTO;
                    }
                    else { return null; }
                }
                else { return null; }
            }
            else { return null; }
        }
        //還沒Debug
        public async Task<IEnumerable<Answer_RecordDTO>> GetRecordAsync(int UserId, int PlanId, int LearnId, int TopicId)
        {
            var User = await _planMemberRepository.GetUserRoleAsync(UserId, PlanId);
            if (User != null)
            {
                var Plan = await _context.BookPlan.FindAsync(PlanId);
                if (Plan != null)
                {
                    var Learn = await _context.Learn.FindAsync(LearnId);
                    if (Learn != null && Plan.Plan_Id == Learn.Plan_Id)
                    {
                        var Topic = await _context.Topic.FindAsync(TopicId);
                        if (Topic != null && Learn.Learn_Id == Topic.Learn_Id)
                        {
                            Answer_Record Record = await _context.Answer_Record.FindAsync(UserId);
                            if (Record != null && Learn.Learn_Id == Record.Learn_Id && Topic.Topic_Id == Record.Topic_Id)
                            {
                                var result = (from a in _context.Answer_Record.
                                              Where(a => a.User_Id == UserId && a.Topic_Id == TopicId)
                                              select new Answer_RecordDTO
                                              {
                                                  User_Id = a.User_Id,
                                                  Topic_Id = a.Topic_Id,
                                                  Learn_Id = a.Learn_Id,
                                                  AnswerDate = a.AnswerDate,
                                                  Answer = a.Answer,
                                                  times = a.times,
                                                  Pass = a.Pass
                                              });
                                return ((IEnumerable<Answer_RecordDTO>)result);
                            }
                            else
                            {
                                return (null);
                            }
                        }
                        else { return (null); }
                    }
                    else
                    {
                        return (null);
                    }
                }
                else
                {
                    return (null);
                }
            }
            else
            {
                return (null);
            }

        }
    }
}
