using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Identity.Client;
using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using System.Linq;
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
        //copy計畫有用到這個，我先保留不動他
        public async Task<IEnumerable<LearnDTO>> GetLearnByChapterIdAsync(int UserId, int PlanId ,int Chapter_Id)
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
        //查詢Learn,加入Chapter_Id
        public async Task<IEnumerable<LearnDTO>> GetLearnByPlanIdAndChapterIdAsync(int UserId, int Chapter_Id)
        {
            var list = new List<LearnDTO>();
            var Learns = await _context.Learn
                .Where(l =>l.Chapter_Id == Chapter_Id)
                .Include(l => l.ProgressTracking)
                .ToListAsync();
            foreach (var a in Learns)
            {
                ProgressTrackingDTO pt = await GetProgressTrack(UserId, a.Learn_Id);
                double PassPersent = await GetPersentOfMemberPass(a.Learn_Id);
                list.Add(new LearnDTO
                {
                    Learn_Id = a.Learn_Id,
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

        public async Task<(LearnDTO, string message)> copylearnAsync(int UserId, int PlanId, int Chapter_Id , LearnDTO InsertData)
        {
            
            BookPlan FindPlan = await _context.BookPlan
                .Include(bp => bp.Learn)
                .SingleOrDefaultAsync(p => p.Plan_Id == PlanId);
            if (FindPlan == null)
            {
                return (null, "錯誤，找不到該計畫");
            }
            var Chapter = await _context.Chapter
                .Where(c =>c.Chapter_Id == Chapter_Id)
                .FirstOrDefaultAsync();
            var lastLearn = await _context.Learn
                .Where(l =>l.Chapter_Id == Chapter_Id)
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
            learn.Chapter_Id = Chapter_Id;
            await _context.Learn.AddAsync(learn);
            await _context.SaveChangesAsync();
            LearnDTO resultDTO = new LearnDTO()
            {
                Learn_Name = learn.Learn_Name,
            };
            await CreateProgressTrackAsync(PlanId, learn.Learn_Id);
            return (resultDTO, "學習內容新增成功");
        }
        public async Task<(LearnDTO, string Message)> CreateLearnAsync(int UserId, int Chapter_Id , LearnDTO InsertData)
        {
            Chapter FindChapter = await _context.Chapter.Where(c => c.Chapter_Id == Chapter_Id).FirstOrDefaultAsync();
            if (FindChapter == null)
            {
                return (null, "找不到該章節");
            }
            BookPlan FindPlan = await _context.BookPlan.Include(bp => bp.Learn).SingleOrDefaultAsync(p => p.Plan_Id == FindChapter.Plan_Id);
            if (FindPlan == null)
            {
                return (null, "錯誤，找不到該計畫");
            }
            var Role = await _planMemberRepository.GetUserRoleAsync(UserId, FindPlan.Plan_Id);
            if (Role != "組長")
            {
                return (null, "錯誤，你不是組長");
            }
  
            var AllLearn = await _context.Learn.Where(l => l.Plan_Id == FindChapter.Plan_Id && l.Chapter_Id == Chapter_Id).ToListAsync();
            foreach (var learns in AllLearn)
            {
                if (InsertData.Learn_Index == learns.Learn_Index) return (null, "錯誤，此學習編號已經存在");
            }
            var defaultDate = DateTime.Now.Date;
            //找出前一個Learn的日期
            var lastLearn = await _context.Learn
                .Where(l => l.Chapter_Id == Chapter_Id)
                .OrderByDescending(l => l.Learn_Index)
                .FirstOrDefaultAsync();
            //前一個沒資料設定
            DateTime previousDate = lastLearn?.DueTime ?? DateTime.Now.Date.AddSeconds(-1);

            //不可超過5項
            var Learns = await _context.Learn.Where(l => l.Chapter_Id == Chapter_Id).CountAsync();
            if(Learns >= 5) { return (null, "錯誤，單一章節不可超過五個學習內容"); }

            Learn learn = new Learn();
            learn.Plan_Id = FindChapter.Plan_Id;
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
            await CreateProgressTrackAsync(FindChapter.Plan_Id , learn.Learn_Id);
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

        public async Task<(LearnDTO, string)> UpdateLearnAsync(
        int userId, int originChapterId, int learnId, LearnDTO dto)
        {
            // ===== 1. 基本驗證 =====
            var originChapter = await _context.Chapter.FindAsync(originChapterId);
            if (originChapter == null) return (null, "錯誤，找不到該章節");

            var role = await _planMemberRepository.GetUserRoleAsync(userId, originChapter.Plan_Id);
            if (role != "組長") return (null, "你沒有權限這麼做");

            var learn = await _context.Learn
                .FirstOrDefaultAsync(l => l.Chapter_Id == originChapterId && l.Learn_Id == learnId);
            if (learn == null) return (null, "錯誤，找不到該學習內容");
            var plan = await _context.Chapter
                .Where(c => c.Chapter_Id == originChapterId)
                .Select(c => c.Plan)
                .FirstOrDefaultAsync();

            // ===== 2. 目標章節與索引 =====
            int targetChapterId = dto.Chapter_Id ?? originChapterId;              
            int targetIndex = dto.Learn_Index;

            // (1) 同章節不可 > 5 筆
            int targetCount = await _context.Learn.CountAsync(l => l.Chapter_Id == targetChapterId);
            if (targetCount >= 5 && targetChapterId != originChapterId)            
                return (null, "錯誤，單一章節不可超過五個學習內容");

            // (2) 先確定目標章節沒有重複 index  (修正問題 #1) 
            bool clash = await _context.Learn.AnyAsync(l =>
                l.Chapter_Id == targetChapterId &&
                l.Learn_Index == targetIndex &&
                l.Learn_Id != learnId);
            if (clash) return (null, "錯誤，此學習編號已經存在");

            // ===== 3. 重新排序：跨章節 or 同章節調整位置 =====
            if (targetChapterId != originChapterId)         
            {
                // 3-1 移除原章節後重排
                var originList = await _context.Learn
                    .Where(l => l.Chapter_Id == originChapterId && l.Learn_Id != learnId)
                    .OrderBy(l => l.Learn_Index)
                    .ToListAsync();
                for (int i = 0; i < originList.Count; i++)
                    originList[i].Learn_Index = i + 1;       

                // 3-2 取目標章節清單，插入指定位置
                var targetList = await _context.Learn
                    .Where(l => l.Chapter_Id == targetChapterId && l.Learn_Id != learnId)
                    .OrderBy(l => l.Learn_Index)
                    .ToListAsync();

                targetIndex = Math.Clamp(targetIndex, 1, targetList.Count + 1); 
                targetList.Insert(targetIndex - 1, learn);

                for (int i = 0; i < targetList.Count; i++)
                    targetList[i].Learn_Index = i + 1;        // 

                learn.Chapter_Id = targetChapterId;           //  更新章節
            }
            else if (targetIndex != learn.Learn_Index)        // ——同章節改順序
            {
                var list = await _context.Learn
                    .Where(l => l.Chapter_Id == originChapterId)
                    .OrderBy(l => l.Learn_Index)
                    .ToListAsync();

                // 移除自己、插回新位置
                list.RemoveAll(l => l.Learn_Id == learnId);
                targetIndex = Math.Clamp(targetIndex, 1, list.Count + 1);
                list.Insert(targetIndex - 1, learn);

                for (int i = 0; i < list.Count; i++)
                    list[i].Learn_Index = i + 1;              // 
            }

            // learn.Learn_Index 已在上面流程更新，不用再手動設；若仍想保險可設一次
            learn.Learn_Index = targetIndex;                  // 

            // ===== 4. 日期相關欄位 =====
            // 重新抓「更新後」的上一個 / 下一個項目 (修正問題 #3) 
            var previousLearn = await _context.Learn
                .Where(l => l.Plan_Id == plan.Plan_Id && l.Learn_Id < learn.Learn_Id)
                .OrderByDescending(l => l.Learn_Id)
                .FirstOrDefaultAsync();

            var nextLearn = await _context.Learn
                .Where(l => l.Plan_Id == plan.Plan_Id && l.Learn_Id > learn.Learn_Id)
                .OrderBy(l => l.Learn_Id)
                .FirstOrDefaultAsync();

            DateTime previousDate = previousLearn?.DueTime ?? DateTime.MinValue.Date;
            DateTime nextDate = nextLearn?.DueTime ?? DateTime.MaxValue.Date;

            // == 沿用你原本的日期運算邏輯 ==
            if (dto.DueTime == DateTime.MinValue)         // 輸入天數模式
            {
                learn.Days = dto.Days;
                learn.DueTime = previousDate.AddDays(dto.Days + 1).AddSeconds(-1);
            }
            else if (dto.DueTime.AddDays(1).AddSeconds(-1) == learn.DueTime)
            {
                learn.DueTime = dto.DueTime;
            }
            else
            {
                if (previousDate == DateTime.MinValue.Date)   // 第一筆
                {
                    learn.DueTime = dto.DueTime.AddDays(1).AddSeconds(-1);
                    var planCreateTime = await _context.PlanMembers
                        .Where(p => p.Plan_Id == originChapter.Plan_Id && p.Role == "組長")
                        .Select(p => p.JoinDate).FirstOrDefaultAsync();
                    learn.Days = (learn.DueTime.Date - planCreateTime.Date).Days;

                    if (nextLearn != null)
                        nextLearn.Days = (nextLearn.DueTime.Date - learn.DueTime.Date).Days;
                }
                else
                {
                    if (dto.DueTime <= previousDate) return (null, "錯誤，日期不可小於前一個計劃");
                    if (dto.DueTime >= nextDate.AddDays(-1)) return (null, "錯誤，日期不可大於下一個計劃");

                    learn.DueTime = dto.DueTime.AddDays(1).AddSeconds(-1);
                    learn.Days = (learn.DueTime - previousDate).Days;

                    if (nextLearn != null)
                        nextLearn.Days = (nextLearn.DueTime - learn.DueTime).Days;
                }
            }

            // ===== 5. 其他欄位 =====
            learn.Learn_Name = dto.Learn_Name;
            learn.Pass_Standard = dto.Pass_Standard;

            // ===== 6. 更新所有 ProgressTracking 的到期日 (修正問題 #5) =====
            var tracks = await _context.ProgressTracking
                .Where(pt => pt.Learn_Id == learnId)
                .ToListAsync();
            foreach (var t in tracks) t.LearnDueTime = learn.DueTime;

            await _context.SaveChangesAsync();

            // ===== 7. 更新計畫所有成員的學習日期 (修正問題 #4) =====
            await UpdateMembersDueTime(originChapter.Plan_Id, targetChapterId);     // 

            return (new LearnDTO { Learn_Name = learn.Learn_Name }, "修改學習內容成功");
        }

        //幫所有成員更新日期
        public async Task UpdateMembersDueTime(int Plan_Id , int Chapter_Id)
        {
            var Members = await _context.PlanMembers
                .Where(p => p.Plan_Id == Plan_Id && p.Role != "組長")
                .ToListAsync();
            var Learns = await _context.Learn
                .Where(l => l.Plan_Id == Plan_Id && l.Chapter_Id == Chapter_Id)
                .ToListAsync();
            foreach(var member in Members)
            {
                foreach(var learn in Learns)
                {
                    var memberProgress = await _context.ProgressTracking
                        .Where(p => p.User_Id == member.User_Id && p.Learn_Id == learn.Learn_Id)
                        .FirstOrDefaultAsync();
                    var previousDate = await _context.ProgressTracking
                        .Include(l => l.Learn)
                        .Where(p => p.User_Id == member.User_Id && p.Learn.Plan_Id == Plan_Id && p.Learn.Learn_Index < learn.Learn_Index)
                        .OrderByDescending(l => l.Learn_Id)
                        .Select(p => (DateTime?)p.LearnDueTime)
                        .FirstOrDefaultAsync();
                    if (previousDate == null)
                    {
                        var JoinDate = await _context.PlanMembers
                            .Where(p => p.User_Id == member.User_Id && p.Plan_Id == Plan_Id)
                            .Select(p => p.JoinDate)
                            .FirstOrDefaultAsync();
                        previousDate = JoinDate.Date.AddDays(1).AddSeconds(-1);
                    }
                    DateTime UseDate = (DateTime)previousDate;
                    memberProgress.LearnDueTime = UseDate.AddDays(learn.Days);

                    await _context.SaveChangesAsync();
                }
            }
        }
        public async Task<(LearnDTO, string Message)> DeleteLearnAsync(int UserId,int Chapter_Id, int Learn_Id)
        {
            var Chapter = await _context.Chapter.FindAsync(Chapter_Id);
            if (Chapter == null) return (null, "錯誤，找不到該章節");
            var Role = await _planMemberRepository.GetUserRoleAsync(UserId, Chapter.Plan_Id);
            if (Role == "組長")
            {
                var DeleteLearn = await _context.Learn.Where(l =>l.Chapter_Id == Chapter_Id).FirstOrDefaultAsync(l => l.Learn_Id == Learn_Id);
                if (DeleteLearn == null)
                {
                    return (null, "找不到該章節的學習");
                }
                var DeleteLearnOfPlan = await _context.BookPlan.FindAsync(Chapter.Plan_Id);
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
                var currentIndex = DeleteLearn.Learn_Index;
                var subsequentLearns = await _context.Learn
                    .Where(l => l.Chapter_Id == Chapter_Id && l.Learn_Index > currentIndex)
                    .ToListAsync();

                foreach (var learn in subsequentLearns)
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



        public async Task<ProgressTrackingDTO> PassProgressAsync(int UserId, int Chapter_Id , int LearnIndex)
        {
            var Chapter = await _context.Chapter.FindAsync(Chapter_Id);
            if (Chapter == null) return null;
            var User = await _planMemberRepository.GetUserRoleAsync(UserId, Chapter.Plan_Id);
            var Plan = await _context.BookPlan.FindAsync(Chapter.Plan_Id);
            var Learn = await _context.Learn.Where(l =>l.Chapter_Id == Chapter_Id).FirstOrDefaultAsync(l => l.Learn_Index == LearnIndex);
            if (User == null && Plan == null && Learn == null)
            {
                return null;
            }
            //找出前一個是否通過
            if (LearnIndex != 1)
            {
                var PreviousLearn = await _context.Learn
                .Where(l =>l.Chapter_Id == Chapter_Id && l.Learn_Index == LearnIndex - 1)
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
        public async Task<IEnumerable<Answer_RecordDTO>> GetRecordAsync(int UserId, int Chapter_Id, int Learn_Index)
        {
            var Chapter = await _context.Chapter.FindAsync(Chapter_Id);
            if (Chapter == null) return null;
            var User = await _planMemberRepository.GetUserRoleAsync(UserId, Chapter.Plan_Id);
            var Plan = await _context.BookPlan.FindAsync(Chapter.Plan_Id);
            var Learn = await _context.Learn
                .Where(l => l.Chapter_Id == Chapter_Id && l.Learn_Index == Learn_Index)
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

        public async Task<List<PassLearnDTO>> GetMemberByPlansAsync(int leaderId, int Chapter_Id, int Learn_Index)
        {
            var Chapter = await _context.Chapter.FindAsync(Chapter_Id);
            if (Chapter == null) return null;
            var IsLeader = await _context.PlanMembers
                .Where(pl => pl.User_Id == leaderId && pl.Plan_Id == Chapter.Plan_Id && pl.Role == "組長")
                .AnyAsync();
            if (!IsLeader)
            {
                return null;
            }

            var learn = await _context.Learn
                .Where(l => l.Plan_Id == Chapter.Plan_Id && l.Learn_Index == Learn_Index)
                .FirstOrDefaultAsync();
            if (learn == null)
            {
                return null;
            }

            var planMembers = await _context.PlanMembers
                .Where(pm => pm.Plan_Id == Chapter.Plan_Id && pm.Role == "組員")
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
