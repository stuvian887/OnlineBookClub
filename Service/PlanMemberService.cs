using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using OnlineBookClub.Repository;
using System.Numerics;

namespace OnlineBookClub.Service
{
    public class PlanMemberService
    {
        private readonly PlanMemberRepository _planMembersRepository;
        private readonly LearnRepository _learnRepository;
        private readonly StatisticService _statisticService;

        public PlanMemberService(PlanMemberRepository planMembersRepository ,LearnRepository learnRepository , StatisticService  statisticService)
        {
            _planMembersRepository = planMembersRepository;
            _learnRepository = learnRepository;
            _statisticService = statisticService;
        }

        public async Task<(bool Success, string Message)> JoinPlanAsync(int UserId , int planid)
        {
            // 1. 檢查計畫是否存在且為公開
            var plan = await _planMembersRepository.GetPlanByIdAsync(planid);
            if (plan == null || !plan.IsPublic)
            {
                return (false, "計畫不存在或不是公開計畫");
            }

            // 2. 檢查是否已經加入
            var isMember = await _planMembersRepository.IsUserInPlanAsync(UserId, planid);
            if (isMember)
            {
                return (false, "你已經加入此計畫");
            }

            // 3. 檢查加入數量限制
            var joinedCount = await _planMembersRepository.GetUserJoinedPlanCountAsync(UserId);
            if (joinedCount >= 100)
            {
                return (false, "你已經加入 100 個計畫，無法再加入");
            }

            // 4. 加入計畫
            await _planMembersRepository.AddUserToPlanAsync(UserId, planid);
                await _statisticService.AddUserCountAsync(planid);
                await _learnRepository.CreateAllProgressTrackAsync(UserId, planid);
            return (true, "成功加入計畫");
        }
        public async Task<(bool Success, string Message)> LeavePlanAsync(int UserId , int planid)
        {
            var role = await _planMembersRepository.GetUserRoleAsync(UserId, planid);
            if (role == null) return (false, "你不是該計畫的成員");

            if (role == "組長") return (false, "組長無法退出計畫，請刪除計畫");

            await _planMembersRepository.RemoveUserFromPlanAsync(UserId, planid);
            return (true, "已成功退出計畫");
        }
        public async Task<bool> IsUserInPlanAsync(int userId, int planId)
        {
            return await _planMembersRepository.IsUserInPlanAsync(userId, planId);
        }

        public async Task<(bool Success, string Message)> CopyPlanAsync(int UserId , int planid)
        {
            var isMember = await _planMembersRepository.IsUserInPlanAsync(UserId, planid);
            if (!isMember) return (false, "你必須先加入此計畫才能複製");

            var newPlanId = await _planMembersRepository.CopyPlanWithoutTopics(planid, UserId);
            return newPlanId > 0 ? (true, "計畫複製成功") : (false, "複製失敗");
        }
        public async Task<List<PlanMembersDTO>>Getmembers(int planid) 
        {
            return await _planMembersRepository.PlanMembersInPlanAsync(planid);
        }
    }
}
