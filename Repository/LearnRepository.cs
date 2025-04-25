using Microsoft.EntityFrameworkCore;
using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using System.Net;
using System.Numerics;
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
            var AllLearnsOfAllPlans = await _context.Learn.ToListAsync();
            if(AllLearnsOfAllPlans != null)
            {
                foreach (var all in AllLearnsOfAllPlans)
                {
                    (string RecentlyLearnDate, string RecentlyLearn) = await GetRecentlyLearn(all.Plan_Id);
                    var result = (from a in _context.Learn
                                  where all.Plan_Id == a.Plan_Id
                                  select new LearnDTO
                                  {
                                      Plan_Id = a.Plan_Id,
                                      Learn_Index = a.Learn_Index,
                                      Learn_Name = a.Learn_Name,
                                      Pass_Standard = a.Pass_Standard,
                                      DueTime = a.DueTime,
                                      RecentlyLearnDate = RecentlyLearnDate,
                                      RecentlyLearn = RecentlyLearn,
                                      Manual_Check = a.Manual_Check,
                                      ProgressTracking = a.ProgressTracking
                                  });
                    var list = await result.ToListAsync();
                    return list.Select(a => GetProgressTrack(a));
                }
                return null;
            }
            else
            {
                return null;
            }
        }
        public async Task<IEnumerable<LearnDTO>> GetLearnByPlanIdAsync(int PlanId)
        {
            (string RecentlyLearnDate , string RecentlyLearn) = await GetRecentlyLearn(PlanId);
            var result = (from a in _context.Learn
                          where PlanId == a.Plan_Id
                          select new LearnDTO
                          {
                              Plan_Id = a.Plan_Id,
                              Learn_Index = a.Learn_Index,
                              Learn_Name = a.Learn_Name,
                              Pass_Standard = a.Pass_Standard,
                              DueTime = a.DueTime,
                              RecentlyLearnDate = RecentlyLearnDate,
                              RecentlyLearn = RecentlyLearn,
                              Manual_Check = a.Manual_Check,
                              ProgressTracking = a.ProgressTracking
                          });
            var list = await result.ToListAsync();
            return list.Select(a => GetProgressTrack(a));
        }
        public async Task<(string , string)> GetRecentlyLearn(int PlanId)
        {
            string TempLearn = "";
            string LearnDate = " ";
            double TempTime = 99999999f;
            var PlanOfLearns = await _context.Learn.Where(l => l.Plan_Id == PlanId).ToListAsync();
            if(PlanOfLearns != null)
            {
                foreach(var learns in PlanOfLearns)
                {
                    System.DateTime NowTime = DateTime.Now;
                    System.TimeSpan FindRecentlyLearnTime = learns.DueTime.Subtract(NowTime);
                    if(FindRecentlyLearnTime.TotalSeconds >= 0 && FindRecentlyLearnTime.TotalSeconds <= TempTime)
                    {
                        TempTime = FindRecentlyLearnTime.TotalSeconds;
                        LearnDate = learns.DueTime.ToString("yyyy/MM/dd");
                        TempLearn = learns.Learn_Name;
                    }
                }
                return (LearnDate , TempLearn);
            }
            else
            {
                return (null,null);
            }
        }
        public async Task<BookPlan> CheckLearnByPlanIdAsync(int PlanId)
        {
            return await _context.BookPlan.Include(bp => bp.Learn).SingleOrDefaultAsync(p => p.Plan_Id == PlanId);
        }
        public async Task<(LearnDTO , string Message)> CreateLearnAsync(int PlanId, LearnDTO InsertData)
        {
            BookPlan FindPlan = await CheckLearnByPlanIdAsync(PlanId);
            if (FindPlan != null)
            {
                var AllLearn = await _context.Learn.Where(l => l.Plan_Id == PlanId).ToListAsync();
                foreach (var learns in AllLearn)
                {
                    if (InsertData.Learn_Index == learns.Learn_Index)
                    {
                        return (null, "錯誤，此學習編號已經存在");
                    }
                }
                System.DateTime NowTime = DateTime.Now;
                System.TimeSpan checkdifftime = InsertData.DueTime.Subtract(NowTime);
                if (checkdifftime.TotalSeconds > 0)
                {
                    Learn learn = new Learn();
                    learn.Plan_Id = PlanId;
                    learn.Learn_Name = InsertData.Learn_Name;
                    learn.Learn_Index = InsertData.Learn_Index;
                    learn.Pass_Standard = InsertData.Pass_Standard;
                    learn.DueTime = InsertData.DueTime.AddDays(1).AddSeconds(-1);
                    await _context.Learn.AddAsync(learn);
                    await _context.SaveChangesAsync();
                    LearnDTO resultDTO = new LearnDTO()
                    {
                        Learn_Name = learn.Learn_Name,
                    };
                    await CreateProgressTrackAsync(PlanId, learn.Learn_Id);
                    return (resultDTO , "學習內容新增成功");
                }
                else
                {
                    return (null, "錯誤，期限不可於今天之前");
                }
            }
            else
            {
                return (null, "錯誤，此學習編號已經存在");
            }
        }
        public async Task<(LearnDTO, string Message)> UpdateLearnAsync(int UserId, int PlanId, int Learn_Index, LearnDTO UpdateData)
        {
            var Role = await _planMemberRepository.GetUserRoleAsync(UserId, PlanId);
            if (Role == "組長")
            {
                var UpdateDataPlan = await _context.BookPlan.FindAsync(PlanId);
                if (UpdateDataPlan != null)
                {
                    var UpdateLearn = await _context.Learn.Where(l => l.Plan_Id == PlanId).FirstOrDefaultAsync(l => l.Learn_Index == Learn_Index);
                    if (UpdateLearn != null)
                    {
                        var AllLearn = await _context.Learn.Where(l => l.Plan_Id == PlanId).ToListAsync();
                        foreach (var learns in AllLearn)
                        {
                            if (UpdateData.Learn_Index == learns.Learn_Index)
                            {
                                return (null, "錯誤，此學習編號已經存在");
                            }
                        }
                        System.DateTime NowTime = DateTime.Now;
                        System.TimeSpan checkdifftime = UpdateData.DueTime.Subtract(NowTime);
                        if (checkdifftime.TotalSeconds > 0)
                        {
                            UpdateLearn.Plan_Id = UpdateDataPlan.Plan_Id;
                            UpdateLearn.Learn_Name = UpdateData.Learn_Name;
                            UpdateLearn.Pass_Standard = UpdateData.Pass_Standard;
                            UpdateLearn.DueTime = UpdateData.DueTime.AddDays(1).AddSeconds(-1);
                            UpdateLearn.Manual_Check = UpdateData.Manual_Check;
                            _context.Update(UpdateLearn);
                            await _context.SaveChangesAsync();
                            LearnDTO resultDTO = new LearnDTO();
                            resultDTO.Learn_Name = UpdateLearn.Learn_Name;
                            return (resultDTO, "修改學習內容成功");
                        }
                        else
                        {
                            return (null, "錯誤，期限不可於今天之前");
                        }
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

        public async Task<(LearnDTO, string Message)> DeleteLearnAsync(int UserId, int PlanId, int Learn_Index)
        {
            var Role = await _planMemberRepository.GetUserRoleAsync(UserId, PlanId);
            if (Role == "組長")
            {
                var DeleteLearnOfPlan = await _context.BookPlan.FindAsync(PlanId);
                if (DeleteLearnOfPlan != null)
                {
                    var DeleteLearn = await _context.Learn.Where(l => l.Plan_Id == PlanId).FirstOrDefaultAsync(l => l.Learn_Index == Learn_Index);
                    if (DeleteLearn != null)
                    {
                        var mardata = await _context.ProgressTracking.Where(X => X.Learn_Id == DeleteLearn.Learn_Id).FirstOrDefaultAsync();
                        _context.ProgressTracking.Remove(mardata);
                        _context.Learn.Remove(DeleteLearn);

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
        public async Task<IEnumerable<ProgressTrackingDTO>> CreateProgressTrackAsync(int PlanId, int Learn_Id)
        {
            List<ProgressTrackingDTO> resultDTOs = new List<ProgressTrackingDTO>();
            var Members = await _context.PlanMembers.Where(pm => pm.Plan_Id == PlanId).ToListAsync();
            if (Members != null)
            {
                foreach (var member in Members)
                {
                    ProgressTracking progress = new ProgressTracking
                    {
                        User_Id = member.User_Id,
                        Learn_Id = Learn_Id,
                        Status = false,
                    };
                    await _context.ProgressTracking.AddAsync(progress);
                    ProgressTrackingDTO resultDTO = new ProgressTrackingDTO
                    {
                        User_Id = progress.User_Id,
                        Learn_Id = Learn_Id,
                        Status = false,
                    };
                    resultDTOs.Add(resultDTO);
                }
                await _context.SaveChangesAsync();
                return resultDTOs;
            }
            else { return null; }
        }
        public async Task<ProgressTrackingDTO> PassProgressAsync(int UserId, int PlanId, int LearnIndex)
        {
            var User = await _planMemberRepository.GetUserRoleAsync(UserId, PlanId);
            if (User != null)
            {
                var Plan = await _context.BookPlan.FindAsync(PlanId);
                if (Plan != null)
                {
                    var Learn = await _context.Learn.Where(l => l.Plan_Id == PlanId).FirstOrDefaultAsync(l => l.Learn_Index == LearnIndex);
                    if (Learn != null)
                    {
                        var PassTheProgress = await _context.ProgressTracking.Where(pt => pt.Learn_Id == Learn.Learn_Id).FirstOrDefaultAsync(pt => pt.Learn_Id == Learn.Learn_Id);
                        PassTheProgress.Status = true;
                        _context.ProgressTracking.Update(PassTheProgress);
                        await _context.SaveChangesAsync();
                        ProgressTrackingDTO resultDTO = new ProgressTrackingDTO
                        {
                            Status = PassTheProgress.Status,
                        };
                        return resultDTO;
                    }
                    return null;
                }
                return null;
            }
            return null;
        }
        public async Task<IEnumerable<Answer_RecordDTO>> CreateRecordAsync(int UserId, AnswerSubmissionDTO submission)
        {
            List<Answer_RecordDTO> resultDTOs = new List<Answer_RecordDTO>();
            var Plan = await _context.BookPlan.FindAsync(submission.Plan_Id);
            if (Plan != null)
            {
                //簡單除錯
                foreach (var answerInput in submission.Answers)
                {
                    int countimes = await _context.Answer_Record.CountAsync(a => a.User_Id == UserId && a.Topic_Id == answerInput.Topic_Id);
                    var topic = await _context.Topic.FindAsync(answerInput.Topic_Id);
                    Answer_Record answer_Record = new Answer_Record();
                    answer_Record.User_Id = UserId;
                    answer_Record.Learn_Id = submission.Learn_Id;
                    answer_Record.Topic_Id = answerInput.Topic_Id;
                    answer_Record.AnswerDate = DateTime.Now;
                    answer_Record.Answer = answerInput.User_Answer;
                    answer_Record.times = countimes + 1;
                    if (answer_Record.Answer == topic.Answer)
                    {
                        answer_Record.Pass = true;
                    }
                    else
                    {
                        answer_Record.Pass = false;
                    }
                    await _context.Answer_Record.AddAsync(answer_Record);
                    Answer_RecordDTO dto = new Answer_RecordDTO
                    {
                        User_Id = answer_Record.User_Id,
                        Learn_Id = answer_Record.Learn_Id,
                        Topic_Id = answer_Record.Topic_Id,
                        AnswerDate = answer_Record.AnswerDate,
                        Answer = answer_Record.Answer,
                        times = answer_Record.times,
                        Pass = answer_Record.Pass
                    };
                    resultDTOs.Add(dto);
                }
                await _context.SaveChangesAsync();
                return resultDTOs;
            }
            else
            {
                return null;
            }
        }
        public async Task<IEnumerable<Answer_RecordDTO>> GetRecordAsync(int UserId, int PlanId, int Learn_Index)
        {
            var User = await _planMemberRepository.GetUserRoleAsync(UserId, PlanId);
            if (User != null)
            {
                var Plan = await _context.BookPlan.FindAsync(PlanId);
                if (Plan != null)
                {
                    var Learn = await _context.Learn.Where(l => l.Plan_Id == PlanId).FirstOrDefaultAsync(l => l.Learn_Index == Learn_Index);
                    if (Learn != null)
                    {
                        var result = (from a in _context.Answer_Record.
                                      Where(a => a.User_Id == UserId)
                                      join b in _context.Topic on a.Topic_Id equals b.Topic_Id
                                      select new Answer_RecordDTO
                                      {
                                          AR_Id = a.AR_Id,
                                          User_Id = a.User_Id,
                                          Topic_Id = a.Topic_Id,
                                          Question_Id = b.Question_Id,
                                          Learn_Id = a.Learn_Id,
                                          AnswerDate = a.AnswerDate,
                                          Answer = a.Answer,
                                          times = a.times,
                                          Pass = a.Pass
                                      });
                        return ((IEnumerable<Answer_RecordDTO>)result);
                    }
                    else { return null; }
                }
                else { return null; }
            }
            else { return (null); }
        }
    }
}
