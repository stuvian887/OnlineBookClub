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
        public async Task<(LearnDTO , string Message)> UpdateLearnAsync(int UserId , int PlanId, int LearnId, LearnDTO UpdateData)
        {
            var Role = await _planMemberRepository.GetUserRoleAsync(UserId, PlanId);
            if(Role == "組長")
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
                        return (resultDTO , "修改計畫成功");
                    }
                    else
                    {
                        return (null , "錯誤，找不到該學習內容");
                    }
                }
                else
                {
                    return (null , "錯誤，找不到該計畫");
                }
            }
            else if(Role == "組員")
            {
                return (null , "你沒有權限這麼做");
            }
            else
            {
                return (null, "找不到你是誰");
            }
        }
            
        public async Task<(LearnDTO , string Message)> DeleteLearnAsync(int UserId , int PlanId, int LearnId)
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
            else if(Role == "組員")
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
    }
}
