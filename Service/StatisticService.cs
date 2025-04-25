using Microsoft.EntityFrameworkCore;
using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using OnlineBookClub.Repository;
using System.Numerics;
namespace OnlineBookClub.Service
{ 
public class StatisticService
{
    private readonly StatisticRepository _repo;

    public StatisticService(StatisticRepository repo)
    {
        _repo = repo;
    }

    public async Task CreateStatistic(int planid)
        {
            var stat = await _repo.GetByPlanIdAsync(planid);
            if (stat == null)
            {
                stat = new Statistic { Plan_Id = planid, CopyCount =0,  UserCount = 0, ViewTimes = 0 };
                await _repo.AddAsync(stat);
            }
          
            await _repo.SaveChangesAsync();
        }
        
    public async Task AddCopyCountAsync(int planId)
    {
        var stat = await _repo.GetByPlanIdAsync(planId);
        if (stat == null)
        {
            stat = new Statistic { Plan_Id = planId, CopyCount = 1, UserCount = 0, ViewTimes = 0 };
            await _repo.AddAsync(stat);
        }
        else
        {
            stat.CopyCount++;
        }
        await _repo.SaveChangesAsync();
    }

    public async Task AddUserCountAsync(int planId)
    {
        var stat = await _repo.GetByPlanIdAsync(planId);
        if (stat == null)
        {
            stat = new Statistic { Plan_Id = planId, CopyCount = 0, UserCount = 1, ViewTimes = 0 };
            await _repo.AddAsync(stat);
        }
        else
        {
            stat.UserCount++;
        }
        await _repo.SaveChangesAsync();
    }

    public async Task AddViewTimesAsync(int planId)
    {
        var stat = await _repo.GetByPlanIdAsync(planId);
        if (stat == null)
        {
            stat = new Statistic { Plan_Id = planId, CopyCount = 0, UserCount = 0, ViewTimes = 1 };
            await _repo.AddAsync(stat);
        }
        else
        {
            stat.ViewTimes++;
        }
        await _repo.SaveChangesAsync();
    }
        public async Task<StatisticDTO?> GetStatisticByPlanIdAsync(int planId)
        {
            var stat = await _repo.GetByPlanIdAsync(planId);
            if (stat == null) return null;

            return new StatisticDTO
            {
                PlanId = stat.Plan_Id,
                CopyCount = stat.CopyCount,
                UserCount = stat.UserCount,
                ViewTimes = stat.ViewTimes
            };
        }

        public async Task<List<StatisticDTO>> GetAllStatisticsAsync()
        {
            var stats = await _repo.GetAll().ToListAsync();

            return stats.Select(s => new StatisticDTO
            {
                PlanId = s.Plan_Id,
                CopyCount = s.CopyCount,
                UserCount = s.UserCount,
                ViewTimes = s.ViewTimes
            }).ToList();
        }
        public async Task DecreaseUserCountAsync(int planId)
        {
            var stat = await _repo.GetByPlanIdAsync(planId);
            if (stat != null && stat.UserCount > 0)
            {
                stat.UserCount--;
                await _repo.SaveChangesAsync();
            }
        }


    }
}