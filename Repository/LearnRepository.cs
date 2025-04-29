using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
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
        //public async Task<IEnumerable<LearnDTO>> GetAllLearnAsync()
        //{
        //    var AllLearnsOfAllPlans = await _context.Learn.ToListAsync();
        //    if (AllLearnsOfAllPlans != null)
        //    {
        //        foreach (var all in AllLearnsOfAllPlans)
        //        {
        //            double PassPersent = await GetPersentOfMemberPass(all.Plan_Id);
        //            (string RecentlyLearnDate, string RecentlyLearn) = await GetRecentlyLearn(all.Plan_Id);
        //            var result = (from a in _context.Learn
        //                          where all.Plan_Id == a.Plan_Id
        //                          select new LearnDTO
        //                          {
        //                              Plan_Id = a.Plan_Id,
        //                              Learn_Index = a.Learn_Index,
        //                              Learn_Name = a.Learn_Name,
        //                              Pass_Standard = a.Pass_Standard,
        //                              DueTime = a.DueTime,
        //                              RecentlyLearnDate = RecentlyLearnDate,
        //                              RecentlyLearn = RecentlyLearn,
        //                              PersentOfMemberPass = PassPersent,
        //                              Manual_Check = a.Manual_Check,
        //                              ProgressTracking = a.ProgressTracking
        //                          });
        //            var list = await result.ToListAsync();
        //        }
        //        return null;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
        public async Task<IEnumerable<LearnDTO>> GetLearnByPlanIdAsync(int PlanId)
        {
            var list = new List<LearnDTO>();
            var Learns = await _context.Learn
                .Where(l => l.Plan_Id == PlanId)
                .Include(l => l.ProgressTracking)
                .ToListAsync();
            foreach (var a in Learns)
            {
                double PassPersent = await GetPersentOfMemberPass(a.Learn_Id);
                list.Add(new LearnDTO
                {
                    Plan_Id = a.Plan_Id,
                    Learn_Index = a.Learn_Index,
                    Learn_Name = a.Learn_Name,
                    Pass_Standard = a.Pass_Standard,
                    DueTime = a.DueTime,
                    PersentOfMemberPass = PassPersent,
                    Manual_Check = a.Manual_Check,
                    ProgressTracking = a.ProgressTracking?.Select(GetProgressTrack).ToList()
                });
            }
            return list;
        }
        public async Task<(IEnumerable<CalendarLearnDTO>, string Message)> GetLearnByCalendar(int UserId, DateTime? BeginTime, DateTime? EndTime)
        {
            if (BeginTime == null)
            {
                return (null, "請輸入起始時間");
            }

            if (EndTime == null)
            {
                return (null, "請輸入結束時間");
            }

            var Progresses = await _context.ProgressTracking
                .Where(p => p.User_Id == UserId)
                .Select(p => p.Learn_Id)
                .Distinct()
                .ToListAsync(); // 抓出這個人的所有 Learn_Id

            DateTime trueStart = BeginTime.Value.Date;
            DateTime trueEnd = EndTime.Value.Date.AddDays(1).AddSeconds(-1);

            var Learns = await _context.Learn
                .Where(learn => Progresses.Contains(learn.Learn_Id) &&
                                learn.DueTime >= trueStart &&
                                learn.DueTime <= trueEnd)
                .Select(learn => new CalendarLearnDTO
                {
                    Plan_Id = learn.Plan_Id,
                    Learn_Name = learn.Learn_Name,
                    DueTime = learn.DueTime,
                })
                .ToListAsync();

            return (Learns, "查詢成功");
        }

        //public async Task<(IEnumerable<CalendarLearnDTO>, string Message)> GetLearnByCalendar(int UserId, DateTime? BeginTime, DateTime? EndTime)
        //{

        //    if (BeginTime == null)
        //    {
        //        return (null, "請輸入起始時間");
        //    }

        //    if (EndTime == null)
        //    {
        //        return (null, "請輸入結束時間");
        //    }

        //    var Progresses = await _context.ProgressTracking.Where(p => p.User_Id == UserId).ToListAsync();
        //    var Learns = await _context.Learn.ToListAsync();
        //    DateTime trueStart = BeginTime.Value.Date;
        //    DateTime trueEnd = EndTime.Value.Date.AddDays(1).AddSeconds(-1);
        //    List<CalendarLearnDTO> resultdto = new List<CalendarLearnDTO>();
        //    foreach (var progress in Progresses)
        //    {
        //        foreach (var learn in Learns)
        //        {
        //            if (learn.DueTime >= trueStart && learn.DueTime <= trueEnd && progress.Learn_Id == learn.Learn_Id)
        //            {
        //                var dto = new CalendarLearnDTO
        //                {
        //                    Plan_Id = learn.Plan_Id,
        //                    Learn_Name = learn.Learn_Name,
        //                    DueTime = learn.DueTime,
        //                };
        //                resultdto.Add(dto);
        //            }
        //        }
        //    }
        //    return (resultdto, "查詢成功");
        //}

        public async Task<double> GetPersentOfMemberPass(int LearnId)
        {
            try
            {
                var Learn = await _context.Learn
                    .Where(l => l.Learn_Id == LearnId)
                    .FirstOrDefaultAsync();
                int PassCount = 0;
                if (Learn == null)
                {
                    return 0;
                }

                var MembersProgress = await _context.ProgressTracking
                    .Where(p => p.Learn_Id == Learn.Learn_Id)
                    .ToListAsync();
                foreach (var MemberPass in MembersProgress)
                {
                    if (MemberPass.Status == true) PassCount++;
                }

                int LearnMemberCount = await _context.ProgressTracking
                    .Where(p => p.Learn_Id == Learn.Learn_Id)
                    .CountAsync();
                double PassPersent = (double)PassCount / LearnMemberCount;
                return PassPersent;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
        }
        //public async Task<(string, string)> GetRecentlyLearn(int PlanId)
        //{
        //    string TempLearn = "";
        //    string LearnDate = " ";
        //    double TempTime = 99999999f;
        //    var PlanOfLearns = await _context.Learn.Where(l => l.Plan_Id == PlanId).ToListAsync();
        //    if (PlanOfLearns != null)
        //    {
        //        foreach (var learns in PlanOfLearns)
        //        {
        //            System.DateTime NowTime = DateTime.Now;
        //            System.TimeSpan FindRecentlyLearnTime = learns.DueTime.Subtract(NowTime);
        //            if (FindRecentlyLearnTime.TotalSeconds >= 0 && FindRecentlyLearnTime.TotalSeconds <= TempTime)
        //            {
        //                TempTime = FindRecentlyLearnTime.TotalSeconds;
        //                LearnDate = learns.DueTime.ToString("yyyy/MM/dd");
        //                TempLearn = learns.Learn_Name;
        //            }
        //        }
        //        return (LearnDate, TempLearn);
        //    }
        //    else
        //    {
        //        return (null, null);
        //    }
        //}
        public async Task<(LearnDTO,string message)> copylearnAsync(int UserId, int PlanId, LearnDTO InsertData)
        {
            BookPlan FindPlan = await _context.BookPlan
                .Include(bp => bp.Learn)
                .SingleOrDefaultAsync(p => p.Plan_Id == PlanId);
            if (FindPlan == null)
            {
                return (null, "錯誤，找不到該計畫");
            }

           

           

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
            return (resultDTO, "學習內容新增成功");
        }
        public async Task<(LearnDTO, string Message)> CreateLearnAsync(int UserId , int PlanId, LearnDTO InsertData)
        {
            var Role = await _planMemberRepository.GetUserRoleAsync(UserId, PlanId);
            if (Role != "組長")
            {
                return (null,"錯誤，你不是組長");
            }
                BookPlan FindPlan = await _context.BookPlan.Include(bp => bp.Learn).SingleOrDefaultAsync(p => p.Plan_Id == PlanId);
            if (FindPlan == null)
            {
                return (null, "錯誤，找不到該計畫");
            }

            var AllLearn = await _context.Learn.Where(l => l.Plan_Id == PlanId).ToListAsync();
            foreach (var learns in AllLearn)
            {
                if (InsertData.Learn_Index == learns.Learn_Index) return (null, "錯誤，此學習編號已經存在");
            }

            System.DateTime NowTime = DateTime.Now.Date;
            System.TimeSpan checkdifftime = InsertData.DueTime.Subtract(NowTime);
            if (checkdifftime.TotalSeconds < 0)
            {
                return (null, "錯誤，期限不可於今天之前");
            }

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
            return (resultDTO, "學習內容新增成功");
        }
        public async Task<(LearnDTO, string Message)> UpdateLearnAsync(int UserId, int PlanId, int Learn_Index, LearnDTO UpdateData)
        {
            var Role = await _planMemberRepository.GetUserRoleAsync(UserId, PlanId);
            if (Role == "組長") 
            {
                var UpdateDataPlan = await _context.BookPlan.FindAsync(PlanId);
                if (UpdateDataPlan == null)
                {
                    return (null, "錯誤，找不到該計畫");
                }

                var UpdateLearn = await _context.Learn.Where(l => l.Plan_Id == PlanId).FirstOrDefaultAsync(l => l.Learn_Index == Learn_Index);
                if (UpdateLearn == null)
                {
                    return (null, "錯誤，找不到該學習內容");
                }

                var AllLearn = await _context.Learn.Where(l => l.Plan_Id == PlanId).ToListAsync();
                //確定使用者真的不會用奇怪的方式讓題號一樣 若不變更題號請不要輸入Learn_Index 不然會做這個if判斷
                foreach (var learns in AllLearn)
                {
                    if (UpdateData.Learn_Index == learns.Learn_Index)
                    {
                        return (null, "錯誤，此學習編號已經存在");
                    }
                }
                System.DateTime NowTime = DateTime.Now.Date;
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

                else return (null, "錯誤，期限不可於今天之前");
            }
            else if (Role == "組員") return (null, "你沒有權限這麼做");
            else return (null, "找不到你是誰");
        }
        public async Task<(LearnDTO, string Message)> DeleteLearnAsync(int UserId, int PlanId, int Learn_Index)
        {
            var Role = await _planMemberRepository.GetUserRoleAsync(UserId, PlanId);
            if (Role == "組長")
            {
                var DeleteLearnOfPlan = await _context.BookPlan.FindAsync(PlanId);
                if (DeleteLearnOfPlan == null)
                {
                    return (null, "找不到此計畫");
                }

                var DeleteLearn = await _context.Learn.Where(l => l.Plan_Id == PlanId).FirstOrDefaultAsync(l => l.Learn_Index == Learn_Index);
                if (DeleteLearn == null)
                {
                    return (null, "找不到此學習");
                }

                var mardata = await _context.ProgressTracking.Where(X => X.Learn_Id == DeleteLearn.Learn_Id).FirstOrDefaultAsync();
                var topic=await _context.Topic.Where(X => X.Learn_Id == DeleteLearn.Learn_Id).FirstOrDefaultAsync();
                _context.ProgressTracking.Remove(mardata);
                _context.Topic.Remove(topic);
                _context.Learn.Remove(DeleteLearn);

                await _context.SaveChangesAsync();
                LearnDTO resultDTO = new LearnDTO();
                resultDTO.Learn_Name = DeleteLearn.Learn_Name;
                return (resultDTO, "刪除成功");
            }
            else if (Role == "組員") return (null, "你沒有權限這麼做");
            else return (null, "找不到你是誰");
        }
        public async Task<LearnDTO> DeleteAllLearnAsync(int PlanId)
        {
            BookPlan DeleteLearnOfPlan = await _context.BookPlan.Include(bp => bp.Learn).SingleOrDefaultAsync(p => p.Plan_Id == PlanId);
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
            else return null;
        }
        public async Task<IEnumerable<MemberProgressDTO>> GetMemberPassLearnPersentAsync(int UserId , int Plan_Id)
        {
            var Role = await _planMemberRepository.GetUserRoleAsync(UserId, Plan_Id);
            if(Role != "組長")
            {
                return null;
            }
            //取得所有成員
            var PlanMembers = await _context.PlanMembers
                .Where(pm => pm.Plan_Id == Plan_Id)
                .Select(pm => new 
                {
                    pm.User_Id,
                    pm.Plan_Id,
                    pm.Role,
                    pm.JoinDate
                }).ToListAsync();

            //取得Learn數量
            var LearnCount = await _context.Learn
                .Where(l => l.Plan_Id == Plan_Id)
                .CountAsync();

            if (LearnCount <= 0) return null;

            List<MemberProgressDTO> result = new List<MemberProgressDTO>();
            foreach(var member in PlanMembers)
            {
                var PassCount = await _context.ProgressTracking
                    .Where(p => p.User_Id == member.User_Id && p.Learn.Plan_Id == Plan_Id && p.Status)
                    .CountAsync();

                double PassPersent = ((double)PassCount / LearnCount) * 100;

                var MemberName = await _context.Members
                    .Where(m => m.User_Id == member.User_Id)
                    .Select(m => m.UserName)
                    .FirstOrDefaultAsync();

                var LearnName = await _context.ProgressTracking
                    .Join(_context.Learn,
                        progress => progress.Learn_Id,
                        learn => learn.Learn_Id,
                        (progress, learn) => new
                        {
                            Progress = progress,
                            Learn = learn,
                        }
                    )
                    .Where(p => p.Progress.User_Id == member.User_Id && p.Learn.Plan_Id == Plan_Id && p.Progress.Status == false)
                    .FirstOrDefaultAsync();

                result.Add(new MemberProgressDTO
                {
                    User_Id = member.User_Id,
                    UserName = MemberName,
                    JoinDate = member.JoinDate,
                    ProgressPercent = PassPersent.ToString(),
                    LearnName = LearnName.Learn.Learn_Name,
                });
            }
            return result;
        }
        public static ProgressTrackingDTO GetProgressTrack(ProgressTracking temp)
        {
            if (temp == null) return null;

            return new ProgressTrackingDTO
            {
                Progress_Id = temp.Progress_Id,
                User_Id = temp.User_Id,
                Learn_Id = temp.Learn_Id,
                Status = temp.Status,
                CompletionDate = temp.CompletionDate,
            };
        }
        public async Task<IEnumerable<ProgressTrackingDTO>> CreateAllProgressTrackAsync(int UserId, int PlanId)
        {
            var Learns = await _context.Learn
                .Where(l => l.Plan_Id == PlanId)
                .ToListAsync();
            if (Learns == null) return null;
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
        public async Task<IEnumerable<ProgressTrackingDTO>> CreateProgressTrackAsync(int PlanId, int Learn_Id)
        {
            List<ProgressTrackingDTO> resultDTOs = new List<ProgressTrackingDTO>();
            var Members = await _context.PlanMembers
                .Where(pm => pm.Plan_Id == PlanId)
                .ToListAsync();
            if (Members == null) return null;
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
        public async Task<ProgressTrackingDTO> PassProgressAsync(int UserId, int PlanId, int LearnIndex)
        {
            var User = await _planMemberRepository.GetUserRoleAsync(UserId, PlanId);
            var Plan = await _context.BookPlan.FindAsync(PlanId);
            var Learn = await _context.Learn.Where(l => l.Plan_Id == PlanId).FirstOrDefaultAsync(l => l.Learn_Index == LearnIndex);
            if (User != null && Plan != null && Learn != null)
            {
                var PassTheProgress = await _context.ProgressTracking.Where(pt => pt.Learn_Id == Learn.Learn_Id).FirstOrDefaultAsync(pt => pt.Learn_Id == Learn.Learn_Id);
                PassTheProgress.Status = true;
                PassTheProgress.CompletionDate = DateTime.Now;
                _context.ProgressTracking.Update(PassTheProgress);
                await _context.SaveChangesAsync();
                ProgressTrackingDTO resultDTO = new ProgressTrackingDTO
                {
                    Status = PassTheProgress.Status,
                    CompletionDate = PassTheProgress.CompletionDate
                };
                return resultDTO;
            }
            else { return null; }
        }

        
        public async Task<IEnumerable<Answer_RecordDTO>> CreateRecordAsync(int UserId, AnswerSubmissionDTO submission)
        {
            List<Answer_RecordDTO> resultDTOs = new List<Answer_RecordDTO>();
            var Plan = await _context.BookPlan.FindAsync(submission.Plan_Id);
            var Learn = await _context.Learn
                .Where(l => l.Plan_Id == submission.Plan_Id)
                .FirstOrDefaultAsync(l => l.Learn_Index == submission.Learn_Index);
            if (Plan == null || Learn == null) { return null; }
            int AnswerCount = 0;
            int PassAnswerCount = 0;
            foreach (var answerInput in submission.Answers)
            {
                var topic = await _context.Topic
                    .FirstOrDefaultAsync(t => t.Learn_Id == Learn.Learn_Id && t.Question_Id == answerInput.Question_Id);
                int countimes = await _context.Answer_Record
                    .CountAsync(a => a.User_Id == UserId && a.Topic_Id == topic.Topic_Id);
                if (topic == null)
                {
                    return null;
                }
                Answer_Record answer_Record = new Answer_Record();
                answer_Record.User_Id = UserId;
                answer_Record.Learn_Id = Learn.Learn_Id;
                answer_Record.Topic_Id = topic.Topic_Id;
                answer_Record.AnswerDate = DateTime.Now;
                answer_Record.Answer = answerInput.User_Answer;
                answer_Record.times = countimes + 1;
                AnswerCount++;
                if (answer_Record.Answer == topic.Answer)
                {
                    answer_Record.Pass = true;
                    PassAnswerCount++;
                }
                else answer_Record.Pass = false;
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
            double CorrectRate = (double)PassAnswerCount / AnswerCount*100;
            if(CorrectRate >= Learn.Pass_Standard)
            {
                var progress = await _context.ProgressTracking
                    .FirstOrDefaultAsync(p => p.User_Id == UserId && p.Learn_Id == Learn.Learn_Id);
                if(progress != null)
                {
                    progress.Status = true;
                    progress.CompletionDate = DateTime.Now;
                }
            }
            await _context.SaveChangesAsync();
            return resultDTOs;
        }
        public async Task<IEnumerable<Answer_RecordDTO>> GetRecordAsync(int UserId, int PlanId, int Learn_Index)
        {
            var User = await _planMemberRepository.GetUserRoleAsync(UserId, PlanId);
            var Plan = await _context.BookPlan.FindAsync(PlanId);
            var Learn = await _context.Learn
                .Where(l => l.Plan_Id == PlanId)
                .FirstOrDefaultAsync(l => l.Learn_Index == Learn_Index);
            if (User != null && Plan != null && Learn != null)
            {
                var result = (from a in _context.Answer_Record
                              .Where(a => a.User_Id == UserId)
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
                return await result.ToListAsync();
            }
            else { return null; }
        }
    }
}
