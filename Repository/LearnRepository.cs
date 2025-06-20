using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Identity.Client;
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
        //copy計畫有用到這個，我先保留不動她他
        public async Task<IEnumerable<LearnDTO>> GetLearnByPlanIdAsync(int UserId, int PlanId)
        {
            var list = new List<LearnDTO>();
            var Learns = await _context.Learn
                .Where(l => l.Plan_Id == PlanId)
                .Include(l => l.ProgressTracking)
                .ToListAsync();
            foreach (var a in Learns)
            {
                ProgressTrackingDTO pt = await GetProgressTrack(UserId, a.Learn_Id);
                double PassPersent = await GetPersentOfMemberPass(a.Learn_Id);
                list.Add(new LearnDTO
                {
                    Plan_Id = a.Plan_Id,
                    Chapter_Id = a.Chapter_Id,
                    Learn_Index = a.Learn_Index,
                    Learn_Name = a.Learn_Name,
                    Pass_Standard = a.Pass_Standard,
                    DueTime = a.DueTime,
                    Days = a.Days,
                    PersentOfMemberPass = PassPersent,
                    Manual_Check = a.Manual_Check,
                    //
                    ProgressTracking = pt != null ? new List<ProgressTrackingDTO> { pt } : new List<ProgressTrackingDTO>()
                });
            }
            return list;
        }
        //查詢Learn,加入Chapter_Id
        public async Task<IEnumerable<LearnDTO>> GetLearnByPlanIdAndChapterIdAsync(int UserId, int PlanId , int Chapter_Id)
        {
            var list = new List<LearnDTO>();
            var Learns = await _context.Learn
                .Where(l => l.Plan_Id == PlanId && l.Chapter_Id == Chapter_Id)
                .Include(l => l.ProgressTracking)
                .ToListAsync();
            foreach (var a in Learns)
            {
                ProgressTrackingDTO pt = await GetProgressTrack(UserId, a.Learn_Id);
                double PassPersent = await GetPersentOfMemberPass(a.Learn_Id);
                list.Add(new LearnDTO
                {
                    Plan_Id = a.Plan_Id,
                    Chapter_Id = a.Chapter_Id,
                    Learn_Index = a.Learn_Index,
                    Learn_Name = a.Learn_Name,
                    Pass_Standard = a.Pass_Standard,
                    DueTime = a.DueTime,
                    Days = a.Days,
                    PersentOfMemberPass = PassPersent,
                    Manual_Check = a.Manual_Check,
                    //
                    ProgressTracking = pt != null ? new List<ProgressTrackingDTO> { pt } : new List<ProgressTrackingDTO>()
                });
            }
            return list;
        }
        public async Task<ProgressTrackingDTO> GetProgressTrack(int UserId, int LearnId)
        {
            var pt = await _context.ProgressTracking
            .FirstOrDefaultAsync(p => p.User_Id == UserId && p.Learn_Id == LearnId);

            if (pt == null) return null;

            return new ProgressTrackingDTO
            {
                Progress_Id = pt.Progress_Id,
                User_Id = pt.User_Id,
                Learn_Id = pt.Learn_Id,
                Status = pt.Status,
                CompletionDate = pt.CompletionDate,
                LearnDueTime = pt.LearnDueTime,
            };

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

            var Learns = await (from learn in _context.Learn
                                join pt in _context.ProgressTracking
                                on learn.Learn_Id equals pt.Learn_Id
                                where pt.User_Id == UserId &&
                                pt.LearnDueTime >= trueStart &&
                                pt.LearnDueTime <= trueEnd
                                select new CalendarLearnDTO
                                {
                                    Plan_Id = learn.Plan_Id,
                                    Learn_Name = learn.Learn_Name,
                                    DueTime = pt.LearnDueTime
                                }).ToListAsync();

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
                    .Include(p => p.User)
                    .ThenInclude(u => u.PlanMembers)
                    .Where(p => p.Learn_Id == Learn.Learn_Id)
                    .Where(p => p.User.PlanMembers.Any(pm => pm.Plan_Id == Learn.Plan_Id && pm.Role != "組長"))
                    .ToListAsync();
                foreach (var MemberPass in MembersProgress)
                {

                    if (MemberPass.Status == true) PassCount++;
                }

                double LearnMemberCount = await _context.ProgressTracking
                    .Include(p => p.User)
                    .ThenInclude(u => u.PlanMembers)
                    .Where(p => p.Learn_Id == Learn.Learn_Id)
                    .Where(p => p.User.PlanMembers.Any(pm => pm.Plan_Id == Learn.Plan_Id && pm.Role != "組長"))
                    .CountAsync();
                if (LearnMemberCount == 0)
                {
                    return 0;
                }
                double PassPersent = Math.Round((double)PassCount / LearnMemberCount, 2) * 100;
                return PassPersent;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
        }

        public async Task<(LearnDTO, string message)> copylearnAsync(int UserId, int PlanId, LearnDTO InsertData)
        {
            BookPlan FindPlan = await _context.BookPlan
                .Include(bp => bp.Learn)
                .SingleOrDefaultAsync(p => p.Plan_Id == PlanId);
            if (FindPlan == null)
            {
                return (null, "錯誤，找不到該計畫");
            }
            var lastLearn = await _context.Learn
                .Where(l => l.Plan_Id == PlanId)
                .OrderByDescending(l => l.Learn_Index)
                .FirstOrDefaultAsync();
            //要找出ProgressTracking的日期
            DateTime previousDate = lastLearn?.DueTime ?? DateTime.Now.Date;

            Learn learn = new Learn();
            learn.Plan_Id = PlanId;
            learn.Learn_Name = InsertData.Learn_Name;
            learn.Learn_Index = InsertData.Learn_Index;
            learn.Pass_Standard = InsertData.Pass_Standard;
            learn.Days = InsertData.Days;
            if (previousDate == DateTime.Now.Date)
            {
                learn.DueTime = previousDate.Date.AddDays(learn.Days).AddSeconds(-1);
            }
            else
            {
                learn.DueTime = previousDate.Date.AddDays(learn.Days + 1).AddSeconds(-1);
            }
            await _context.Learn.AddAsync(learn);
            await _context.SaveChangesAsync();
            LearnDTO resultDTO = new LearnDTO()
            {
                Learn_Name = learn.Learn_Name,
            };
            await CreateProgressTrackAsync(PlanId, learn.Learn_Id);
            return (resultDTO, "學習內容新增成功");
        }
        public async Task<(LearnDTO, string Message)> CreateLearnAsync(int UserId, int PlanId,int Chapter_Id , LearnDTO InsertData)
        {
            var Role = await _planMemberRepository.GetUserRoleAsync(UserId, PlanId);
            if (Role != "組長")
            {
                return (null, "錯誤，你不是組長");
            }
            BookPlan FindPlan = await _context.BookPlan.Include(bp => bp.Learn).SingleOrDefaultAsync(p => p.Plan_Id == PlanId);
            if (FindPlan == null)
            {
                return (null, "錯誤，找不到該計畫");
            }
            Chapter FindChapter = await _context.Chapter.Where(c => c.Chapter_Id == Chapter_Id).FirstOrDefaultAsync();
            if (FindChapter == null)
            {
                return (null, "找不到該章節");
            }

            var AllLearn = await _context.Learn.Where(l => l.Plan_Id == PlanId && l.Chapter_Id == Chapter_Id).ToListAsync();
            foreach (var learns in AllLearn)
            {
                if (InsertData.Learn_Index == learns.Learn_Index) return (null, "錯誤，此學習編號已經存在");
            }
            var defaultDate = DateTime.Now.Date;
            //找出前一個Learn的日期
            var lastLearn = await _context.Learn
                .Where(l => l.Plan_Id == PlanId && l.Chapter_Id == Chapter_Id)
                .OrderByDescending(l => l.Learn_Index)
                .FirstOrDefaultAsync();
            //前一個沒資料 是以該天00:00:00計算
            DateTime previousDate = lastLearn?.DueTime ?? DateTime.Now.Date.AddDays(1).AddSeconds(-1);

            //不可超過5項
            var Learns = await _context.Learn.Where(l => l.Chapter_Id == Chapter_Id).CountAsync();
            if(Learns >= 5) { return (null, "錯誤，單一章節不可超過五個學習內容"); }

            Learn learn = new Learn();
            learn.Plan_Id = PlanId;
            learn.Chapter_Id = Chapter_Id;
            learn.Learn_Name = InsertData.Learn_Name;
            learn.Learn_Index = InsertData.Learn_Index;
            learn.Pass_Standard = InsertData.Pass_Standard;
            //使用者可以輸入日期 會自動轉換成天數 也可自行填入天數
            if (InsertData.DueTime == DateTime.MinValue)
            {
                learn.Days = InsertData.Days;
                learn.DueTime = defaultDate.AddDays(learn.Days + 1).AddMinutes(-1);
            }
            else
            {
                System.DateTime NowTime = DateTime.Now.Date;
                System.TimeSpan checkdifftime = InsertData.DueTime.Subtract(NowTime);
                if (checkdifftime.TotalSeconds < 0)
                {
                    return (null, "錯誤，期限不可於今天之前");
                }
                else if (InsertData.DueTime <= previousDate)
                {
                    return (null, "錯誤，期限小於前一個計畫");
                }
                learn.DueTime = InsertData.DueTime.AddDays(1).AddSeconds(-1);
                //+1刪除
                learn.Days = (InsertData.DueTime.AddDays(1) - previousDate).Days;
            }

            await _context.Learn.AddAsync(learn);
            await _context.SaveChangesAsync();
            LearnDTO resultDTO = new LearnDTO()
            {
                Learn_Name = learn.Learn_Name,
            };
            await CreateProgressTrackAsync(PlanId , learn.Learn_Id);
            return (resultDTO, "學習內容新增成功");
        }
        //創建新Learn時幫所有成員新增ProgressTracking
        public async Task<IEnumerable<ProgressTrackingDTO>> CreateProgressTrackAsync(int PlanId, int Learn_Id)
        {
            List<ProgressTrackingDTO> resultDTOs = new List<ProgressTrackingDTO>();
            var Members = await _context.PlanMembers
                .Where(pm => pm.Plan_Id == PlanId)
                .ToListAsync();
            var Learn = await _context.Learn
                .Where(l => l.Learn_Id == Learn_Id)
                .FirstOrDefaultAsync();

            if (Members == null) return null;
            foreach (var member in Members)
            {
                var lastDueTime = await _context.ProgressTracking
                    .Include(pt => pt.Learn)
                        .ThenInclude(l => l.Plan)
                    .Where(pt => pt.User_Id == member.User_Id && pt.Learn.Plan.Plan_Id == PlanId)
                    .OrderByDescending(p => p.LearnDueTime)
                    .Select(p => (DateTime?)p.LearnDueTime)
                    .FirstOrDefaultAsync();

                DateTime TheDueTime = DateTime.MaxValue;
                if(member.Role == "組長")
                {
                    TheDueTime = Learn.DueTime;
                }
                else if (lastDueTime.HasValue)
                {
                    // 重點：用前次 DueTime 當基準
                    TheDueTime = lastDueTime.Value.AddDays(Learn.Days).AddSeconds(-1);
                }
                else
                {
                    // 第一次新增，用現在日期當基準
                    TheDueTime = DateTime.Now.Date.AddDays(Learn.Days).AddSeconds(-1);
                }

                ProgressTracking progress = new ProgressTracking
                {
                    User_Id = member.User_Id,
                    Learn_Id = Learn_Id,
                    Status = false,
                    LearnDueTime = TheDueTime,
                };
                await _context.ProgressTracking.AddAsync(progress);
                ProgressTrackingDTO resultDTO = new ProgressTrackingDTO
                {
                    User_Id = progress.User_Id,
                    Learn_Id = Learn_Id,
                    Status = false,
                    LearnDueTime = progress.LearnDueTime,
                };
                resultDTOs.Add(resultDTO);
            }
            await _context.SaveChangesAsync();
            return resultDTOs;
        }
        //加入計畫後創建ProgressTracking
        public async Task<IEnumerable<ProgressTrackingDTO>> CreateAllProgressTrackAsync(int UserId, int PlanId)
        {
            var Learns = await _context.Learn
                .Where(l => l.Plan_Id == PlanId)
                .ToListAsync();
            if (Learns == null) return null;
            List<ProgressTrackingDTO> resultDTOs = new List<ProgressTrackingDTO>();
            DateTime TempDate = DateTime.Now.Date;
            foreach (var learn in Learns)
            {
                DateTime TheDueTime = DateTime.MinValue.Date;
                //防止Days是0卻會增加的狀況發生
                if (learn.Days == 0)
                {
                    TheDueTime = TempDate.AddDays(learn.Days);
                    TempDate = TheDueTime;
                }
                //第一筆加一天調整
                if (learn.Learn_Index == 1)
                {
                    TheDueTime = TempDate.AddDays(learn.Days + 1).AddSeconds(-1);
                    TempDate = TheDueTime.AddSeconds(1);
                }
                else
                {
                    TheDueTime = TempDate.AddDays(learn.Days).AddSeconds(-1);
                    TempDate = TheDueTime.AddSeconds(1);
                }
                ProgressTracking progress = new ProgressTracking
                {
                    User_Id = UserId,
                    Learn_Id = learn.Learn_Id,
                    Status = false,
                    LearnDueTime = TheDueTime,
                };
                await _context.ProgressTracking.AddAsync(progress);
                ProgressTrackingDTO dto = new ProgressTrackingDTO
                {
                    User_Id = progress.User_Id,
                    Learn_Id = progress.Learn_Id,
                    Status = progress.Status,
                    LearnDueTime = progress.LearnDueTime,
                };
                resultDTOs.Add(dto);
            }
            await _context.SaveChangesAsync();
            return resultDTOs;
        }

        public async Task<(LearnDTO, string Message)> UpdateLearnAsync(int UserId, int PlanId, int Chapter_Id, int Learn_Index,  LearnDTO UpdateData)
        {
            var Role = await _planMemberRepository.GetUserRoleAsync(UserId, PlanId);
            if (Role == "組長")
            {
                var UpdateDataPlan = await _context.BookPlan.FindAsync(PlanId);
                if (UpdateDataPlan == null)
                {
                    return (null, "錯誤，找不到該計畫");
                }

                var UpdateLearn = await _context.Learn.Where(l => l.Plan_Id == PlanId && l.Chapter_Id == Chapter_Id).FirstOrDefaultAsync(l => l.Learn_Index == Learn_Index);
                if (UpdateLearn == null)
                {
                    return (null, "錯誤，找不到該學習內容");
                }
                var AllLearn = await _context.Learn.Where(l => l.Plan_Id == PlanId && l.Chapter_Id == Chapter_Id).ToListAsync();
                //確定編號不會重複
                bool isIndexExist = await _context.Learn
                    .AnyAsync(l => l.Plan_Id == PlanId && l.Chapter_Id == Chapter_Id
                    && l.Learn_Index == UpdateData.Learn_Index
                    && l.Learn_Index != Learn_Index);
                if (isIndexExist)
                {
                    return (null, "錯誤，此學習編號已經存在");
                }
                //創建計畫時間
                var PlanCreateTime = await _context.PlanMembers
                    .Where(p => p.Plan_Id == PlanId && p.Role == "組長")
                    .Select(p => p.JoinDate)
                    .FirstOrDefaultAsync();

                //不可超過5項
                var Learns = await _context.Learn.Where(l => l.Chapter_Id == UpdateData.Chapter_Id).CountAsync();
                if (Learns >= 5) { return (null, "錯誤，單一章節不可超過五個學習內容"); }
                // 動態查詢前一個學習計畫（Learn_Index 小於當前值中的最大者）
                var previousLearn = await _context.Learn
                    .Where(l => l.Plan_Id == PlanId && l.Chapter_Id == Chapter_Id
                             && l.Learn_Index < UpdateLearn.Learn_Index)
                    .OrderByDescending(l => l.Learn_Index)
                    .FirstOrDefaultAsync();
                //要找出上一個跟下一個Learn
                var nextLearn = await _context.Learn
                    .Where(l => l.Plan_Id == PlanId && l.Chapter_Id == Chapter_Id
                             && l.Learn_Index > UpdateLearn.Learn_Index)
                    .OrderBy(l => l.Learn_Index)
                    .FirstOrDefaultAsync();
                // 基準日期：前一個計畫的 DueTime，若無則用系統預設最小日期先帶入
                DateTime previousDate = previousLearn?.DueTime ?? DateTime.MinValue.Date;
                // 後一個計劃的DueTime
                DateTime nextDate = nextLearn?.DueTime ?? DateTime.MaxValue.Date;

                // 更新邏輯 //第一個判斷為輸入天數模式，但我們系統沒用到，所以不管他
                if (UpdateData.DueTime == DateTime.MinValue)
                {
                    // 輸入天數模式
                    UpdateLearn.Days = UpdateData.Days;
                    UpdateLearn.DueTime = previousDate.AddDays(UpdateData.Days + 1).AddSeconds(-1);
                }
                //日期沒動
                else if (UpdateData.DueTime.AddDays(1).AddSeconds(-1) == UpdateLearn.DueTime)
                {
                    UpdateLearn.DueTime = UpdateData.DueTime;
                }
                else
                {
                    //如果為第一筆
                    if(previousDate == DateTime.MinValue.Date)
                    {
                        UpdateLearn.DueTime = UpdateData.DueTime.AddDays(1).AddSeconds(-1);
                        //原本用改動計畫時間，改為用創建計畫時間
                        UpdateLearn.Days = (UpdateLearn.DueTime.Date - PlanCreateTime.Date).Days;
                        if (nextLearn != null)
                        {
                            nextLearn.Days = (nextLearn.DueTime.Date - UpdateLearn.DueTime.Date).Days;
                        }
                    }
                    else
                    {
                        if (UpdateData.DueTime <= previousDate)
                        {
                            return (null, "錯誤，日期不可小於前一個計劃");
                        }
                        if (UpdateData.DueTime >= nextDate.AddDays(-1))
                        {
                            return (null, "錯誤，日期不可大於下一個計劃");
                        }
                        // 輸入日期模式
                        UpdateLearn.DueTime = UpdateData.DueTime.AddDays(1).AddSeconds(-1);
                        UpdateLearn.Days = (UpdateLearn.DueTime - previousDate).Days;
                        if (nextLearn != null)
                        {
                            nextLearn.Days = (nextLearn.DueTime - UpdateLearn.DueTime).Days;
                        }
                    }
                }
                if(UpdateData.Chapter_Id == null)
                {
                    UpdateLearn.Chapter_Id = Chapter_Id;
                }
                else
                {
                    UpdateLearn.Chapter_Id = UpdateData.Chapter_Id;
                }
                UpdateLearn.Learn_Index = UpdateData.Learn_Index;
                UpdateLearn.Learn_Name = UpdateData.Learn_Name;
                UpdateLearn.Pass_Standard = UpdateData.Pass_Standard;
                _context.Update(UpdateLearn);

                //讓ProgressTrackingTracking的日期也更動
                var UserProgress = await _context.ProgressTracking
                    .Where(pt => pt.Learn_Id == UpdateLearn.Learn_Id && pt.User_Id == UserId)
                    .FirstOrDefaultAsync();
                if (UserProgress != null)
                {
                    UserProgress.LearnDueTime = UpdateLearn.DueTime;
                }

                await _context.SaveChangesAsync();
                //變更成員的學習日期
                await UpdateMembersDueTime(UserId ,UpdateLearn, previousLearn, nextLearn);
                LearnDTO resultDTO = new LearnDTO();
                resultDTO.Learn_Name = UpdateLearn.Learn_Name;
                return (resultDTO, "修改學習內容成功");

            }
            else if (Role == "組員") return (null, "你沒有權限這麼做");
            else return (null, "找不到你是誰");
        }
        //幫所有成員更新日期
        public async Task UpdateMembersDueTime(int UserId ,Learn currentLearn , Learn previousLearn , Learn nextLearn)
        {
            //找出該成員的加入時間
            var Members = await _context.ProgressTracking
                .Where(p => p.Learn_Id == currentLearn.Learn_Id && p.User_Id != UserId)
                .ToListAsync();
            foreach(var member in Members)
            {
                var memberJoinDate = await _context.PlanMembers
                    .Where(p => p.Plan_Id == currentLearn.Plan_Id && p.User_Id == member.User_Id)
                    .Select(p => p.JoinDate)
                    .FirstOrDefaultAsync();
                //找出當前進度
                var currentProgress = await _context.ProgressTracking
                    .Where(p => p.Learn_Id == currentLearn.Learn_Id && p.User_Id == member.User_Id)
                    .FirstOrDefaultAsync();
                var previousDate = DateTime.MinValue;
                if (previousLearn != null)
                {
                    previousDate = await _context.ProgressTracking
                    .Where(p => p.Learn_Id == previousLearn.Learn_Id && p.User_Id == member.User_Id)
                    .Select(p => p.LearnDueTime)
                    .FirstOrDefaultAsync();
                }

                //如果更新的是第一筆， 用他的加入時間去算
                if (previousDate == DateTime.MinValue)
                {
                    member.LearnDueTime = memberJoinDate.Date.AddDays(currentLearn.Days + 1).AddMinutes(-1);
                }
                else
                {
                    member.LearnDueTime = previousDate.AddDays(currentLearn.Days);
                }
            }
            await _context.SaveChangesAsync();
        }
        public async Task<(LearnDTO, string Message)> DeleteLearnAsync(int UserId, int PlanId,int Chapter_Id, int Learn_Index)
        {
            var Role = await _planMemberRepository.GetUserRoleAsync(UserId, PlanId);
            if (Role == "組長")
            {
                var DeleteLearn = await _context.Learn.Where(l => l.Plan_Id == PlanId && l.Chapter_Id == Chapter_Id).FirstOrDefaultAsync(l => l.Learn_Index == Learn_Index);
                if (DeleteLearn == null)
                {
                    return (null, "找不到該章節的學習");
                }
                var DeleteLearnOfPlan = await _context.BookPlan.FindAsync(PlanId);
                if (DeleteLearnOfPlan == null)
                {
                    return (null, "找不到此計畫");
                }
                var mardata = await _context.ProgressTracking.Where(X => X.Learn_Id == DeleteLearn.Learn_Id).FirstOrDefaultAsync();
                var topic = await _context.Topic.Where(X => X.Learn_Id == DeleteLearn.Learn_Id).FirstOrDefaultAsync();
                if (mardata != null)
                {
                    _context.ProgressTracking.Remove(mardata);
                }
                if (topic != null)
                {
                    _context.Topic.Remove(topic);
                }
                _context.Learn.Remove(DeleteLearn);
                //變更編號邏輯
                var subsequentLearns = await _context.Learn
                    .Where(l => l.Plan_Id == PlanId && l.Learn_Index > Learn_Index)
                    .ToListAsync();
                foreach(var learn in subsequentLearns)
                {
                    learn.Learn_Index -= 1;
                }

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
        public async Task<IEnumerable<MemberProgressDTO>> GetMemberPassLearnPersentAsync(int UserId, int Plan_Id)
        {
            var Role = await _planMemberRepository.GetUserRoleAsync(UserId, Plan_Id);
            if (Role != "組長")
            {
                return null;
            }
            //取得所有成員
            var PlanMembers = await _context.PlanMembers
                .Where(pm => pm.Plan_Id == Plan_Id && pm.User_Id != UserId)
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
            foreach (var member in PlanMembers)
            {
                var PassCount = await _context.ProgressTracking
                    .Where(p => p.User_Id == member.User_Id && p.Learn.Plan_Id == Plan_Id && p.Status)
                    .CountAsync();

                double PassPersent = ((double)PassCount / LearnCount) * 100;

                var MemberName = await _context.Members
                    .Where(m => m.User_Id == member.User_Id)
                    .Select(m => m.UserName)
                    .FirstOrDefaultAsync();

                //先where在join
                var LearnName = await _context.ProgressTracking
                    .Where(p => p.User_Id == member.User_Id && p.Learn.Plan_Id == Plan_Id && p.Status == false)
                    .Join(_context.Learn,
                        progress => progress.Learn_Id,
                        learn => learn.Learn_Id,
                        (progress, learn) => new
                        {
                            Progress = progress,
                            Learn = learn,
                        }
                    )
                    .FirstOrDefaultAsync();

                result.Add(new MemberProgressDTO
                {
                    User_Id = member.User_Id,
                    UserName = MemberName,
                    JoinDate = member.JoinDate.Date.ToString("yyyy/MM/dd"),
                    ProgressPercent = PassPersent.ToString(),
                    LearnName = LearnName?.Learn.Learn_Name ?? "全部完成!",
                    IsComplete = LearnName?.Progress.Status ?? true
                });
            }
            return result;
        }



        public async Task<ProgressTrackingDTO> PassProgressAsync(int UserId, int PlanId,int Chapter_Id , int LearnIndex)
        {
            var User = await _planMemberRepository.GetUserRoleAsync(UserId, PlanId);
            var Plan = await _context.BookPlan.FindAsync(PlanId);
            var Chapter = await _context.Chapter.FindAsync(Chapter_Id);
            var Learn = await _context.Learn.Where(l => l.Plan_Id == PlanId && l.Chapter_Id == Chapter_Id).FirstOrDefaultAsync(l => l.Learn_Index == LearnIndex);
            if (User == null && Plan == null && Chapter == null && Learn == null)
            {
                return null;
            }
            //找出前一個是否通過
            if (LearnIndex != 1)
            {
                var PreviousLearn = await _context.Learn
                .Where(l => l.Plan_Id == PlanId && l.Chapter_Id == Chapter_Id && l.Learn_Index == LearnIndex - 1)
                .FirstOrDefaultAsync();
                var CheckIsCanPass = await _context.ProgressTracking
                .Where(pt => pt.User_Id == UserId && pt.Learn_Id == PreviousLearn.Learn_Id)
                .FirstOrDefaultAsync();
                if (CheckIsCanPass.Status == false)
                {
                    return null;
                }
            }
            var PassTheProgress = await _context.ProgressTracking.Where(pt => pt.User_Id == UserId).FirstOrDefaultAsync(pt => pt.Learn_Id == Learn.Learn_Id);
            PassTheProgress.Status = true;
            PassTheProgress.CompletionDate = DateTime.Now;
            _context.ProgressTracking.Update(PassTheProgress);
            await _context.SaveChangesAsync();
            ProgressTrackingDTO resultDTO = new()
            {
                Status = PassTheProgress.Status,
                CompletionDate = PassTheProgress.CompletionDate
            };
            return resultDTO;
        }


        public async Task<IEnumerable<Answer_RecordDTO>> CreateRecordAsync(int UserId, AnswerSubmissionDTO submission)
        {
            List<Answer_RecordDTO> resultDTOs = new List<Answer_RecordDTO>();
            var Plan = await _context.BookPlan.FindAsync(submission.Plan_Id);
            var Learn = await _context.Learn
                .Where(l => l.Plan_Id == submission.Plan_Id)
                .FirstOrDefaultAsync(l => l.Learn_Index == submission.Learn_Index);
            if (Plan == null || Learn == null) { return null; }
            int AnswerCount = await _context.Topic
                .Where(t => t.Learn_Id == Learn.Learn_Id)
                .CountAsync();
            int PassAnswerCount = 0;
            foreach (var answerInput in submission.Answers)
            {
                var topic = await _context.Topic
                    .FirstOrDefaultAsync(t => t.Learn_Id == Learn.Learn_Id && t.Question_Id == answerInput.Question_Id);
                if (topic == null)
                {
                    return null;
                }
                int countimes = await _context.Answer_Record
                    .CountAsync(a => a.User_Id == UserId && a.Topic_Id == topic.Topic_Id);

                Answer_Record answer_Record = new Answer_Record();
                answer_Record.User_Id = UserId;
                answer_Record.Learn_Id = Learn.Learn_Id;
                answer_Record.Topic_Id = topic.Topic_Id;
                answer_Record.AnswerDate = DateTime.Now;
                answer_Record.Answer = answerInput.User_Answer;
                answer_Record.times = countimes + 1;
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
            double CorrectRate = (double)PassAnswerCount / AnswerCount;
            if (CorrectRate >= Learn.Pass_Standard)
            {
                var progress = await _context.ProgressTracking
                    .FirstOrDefaultAsync(p => p.User_Id == UserId && p.Learn_Id == Learn.Learn_Id);
                if (progress != null)
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
                .Where(l => l.Plan_Id == PlanId && l.Learn_Index == Learn_Index)
                .FirstOrDefaultAsync();
            if (User != null && Plan != null && Learn != null)
            {
                var result = (from a in _context.Answer_Record
                              .Where(a => a.User_Id == UserId && a.Learn_Id == Learn.Learn_Id)
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

        public async Task<List<PassLearnDTO>> GetMemberByPlansAsync(int leaderId, int planId, int Learn_Index)
        {
            var IsLeader = await _context.PlanMembers
                .Where(pl => pl.User_Id == leaderId && pl.Plan_Id == planId && pl.Role == "組長")
                .AnyAsync();
            if (!IsLeader)
            {
                return null;
            }

            var learn = await _context.Learn
                .Where(l => l.Plan_Id == planId && l.Learn_Index == Learn_Index)
                .FirstOrDefaultAsync();
            if (learn == null)
            {
                return null;
            }

            var planMembers = await _context.PlanMembers
                .Where(pm => pm.Plan_Id == planId && pm.Role == "組員")
                .ToListAsync();

            var dtoList = new List<PassLearnDTO>();
            foreach (var planMember in planMembers)
            {
                var member = await _context.Members
                    .Where(m => m.User_Id == planMember.User_Id)
                    .FirstOrDefaultAsync();

                var Times = await _context.Answer_Record
                    .Where(a => a.User_Id == planMember.User_Id && a.Learn_Id == learn.Learn_Id)
                    .OrderByDescending(a => a.times)
                    .FirstOrDefaultAsync();

                var complete = await _context.ProgressTracking
                    .Where(p => p.User_Id == planMember.User_Id && p.Learn_Id == learn.Learn_Id)
                    .FirstOrDefaultAsync();

                dtoList.Add(new PassLearnDTO
                {
                    UserName = member?.UserName ?? "未知使用者",
                    times = Times?.times ?? 0,
                    IsComplete = complete?.Status ?? false
                });
            }
            return dtoList;
        }
        public void MoveLearnToChapter()
        {
            var bookPlans = _context.BookPlan
            .Include(bp => bp.Learn) // ← 確保可以拿到該 BookPlan 底下所有 Learn
            .ToList();

            foreach (var plan in bookPlans)
            {
                // 1. 建立一個預設章節
                var chapter = new Chapter
                {
                    Plan_Id = plan.Plan_Id,
                    Chapter_Index = 1,
                    Chapter_Name = "預設章節"
                };

                _context.Chapter.Add(chapter);
                _context.SaveChanges(); 

                // 2. 將這個計畫下的所有 Learn 都指定到剛剛新增的章節
                foreach (var learn in plan.Learn)
                {
                    learn.Chapter_Id = chapter.Chapter_Id;
                }

                _context.SaveChanges(); 
            }
        }

    }
}
