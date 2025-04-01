using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using OnlineBookClub.DTO;
using OnlineBookClub.Models;

namespace OnlineBookClub.Repository
{
    public class LearnRepository
    {
        private readonly OnlineBookClubContext _context;
        public LearnRepository(OnlineBookClubContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<LearnDTO>> GetAllLearn()
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
                          });
            return await result.ToListAsync();
        }
        public async Task<IEnumerable<LearnDTO>> GetLearnByPlanId(int PlanId)
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
            return await result.ToListAsync();
        }
        public async Task<Learn> GetLearnByLearnId(int LearnId)
        {
            var result = await _context.Learn.SingleOrDefaultAsync(l => l.Learn_Id == LearnId);
            return result;
        }
        public async Task<BookPlan> CheckLearnByPlanId(int PlanId)
        {
            var result = await _context.BookPlan.SingleOrDefaultAsync(p => p.Plan_Id == PlanId);
            return result;
        }

        public async Task<LearnDTO> CreateLearn(int PlanId, LearnDTO InsertData)
        {
            BookPlan FindPlan = await CheckLearnByPlanId(PlanId);
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
                return resultDTO;
            }
            else
            {
                return null;
            }
        }
        public async Task<LearnDTO> UpdateLearn(int PlanId , int LearnId, LearnDTO UpdateData)
        {
            BookPlan UpdateDataPlan = await CheckLearnByPlanId(PlanId);
            if(UpdateDataPlan != null)
            {
                Learn UpdateLearn = await GetLearnByLearnId(LearnId);
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
        public async Task<LearnDTO> DeleteLearn(int PlanId , int LearnId)
        {
            BookPlan DeleteLearnOfPlan = await CheckLearnByPlanId(PlanId);
            if (DeleteLearnOfPlan != null)
            {
                Learn DeleteLearn = await GetLearnByLearnId(LearnId);
                if (DeleteLearn != null && DeleteLearn.Plan_Id == PlanId)
                {
                    _context.Remove(DeleteLearn);
                    await _context.SaveChangesAsync();
                    LearnDTO resultDTO = new LearnDTO();
                    resultDTO.Learn_Name = DeleteLearn.Learn_Name;
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
    }
}
