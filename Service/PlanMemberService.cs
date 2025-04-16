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

        public async Task<(bool Success, string Message)> JoinPlanAsync(PlanMembers joinPlanDto)
        {
            // 1. 檢查計畫是否存在且為公開
            var plan = await _planMembersRepository.GetPlanByIdAsync(joinPlanDto.Plan_Id);
            if (plan == null || !plan.IsPublic)
            {
                return (false, "計畫不存在或不是公開計畫");
            }

            // 2. 檢查是否已經加入
            var isMember = await _planMembersRepository.IsUserInPlanAsync(joinPlanDto.User_Id, joinPlanDto.Plan_Id);
            if (isMember)
            {
                return (false, "你已經加入此計畫");
            }

            // 3. 檢查加入數量限制
            var joinedCount = await _planMembersRepository.GetUserJoinedPlanCountAsync(joinPlanDto.User_Id);
            if (joinedCount >= 100)
            {
                return (false, "你已經加入 100 個計畫，無法再加入");
            }

            // 4. 加入計畫
            await _planMembersRepository.AddUserToPlanAsync(joinPlanDto.User_Id, joinPlanDto.Plan_Id);
            await _statisticService.AddUserCountAsync(joinPlanDto.Plan_Id);
            await _learnRepository.CreateAllProgressTrackAsync(joinPlanDto.User_Id, joinPlanDto.Plan_Id);
            return (true, "成功加入計畫");
        }
        public async Task<(bool Success, string Message)> LeavePlanAsync(PlanMembers leavePlanDto)
        {
            var role = await _planMembersRepository.GetUserRoleAsync(leavePlanDto.User_Id, leavePlanDto.Plan_Id);
            if (role == null) return (false, "你不是該計畫的成員");

            if (role == "組長") return (false, "組長無法退出計畫，請刪除計畫");

            await _planMembersRepository.RemoveUserFromPlanAsync(leavePlanDto.User_Id, leavePlanDto.Plan_Id);
            return (true, "已成功退出計畫");
        }
        public async Task<bool> IsUserInPlanAsync(int userId, int planId)
        {
            return await _planMembersRepository.IsUserInPlanAsync(userId, planId);
        }

        public async Task<(bool Success, string Message)> CopyPlanAsync(PlanMembers copyPlanDto)
        {
            var isMember = await _planMembersRepository.IsUserInPlanAsync(copyPlanDto.User_Id, copyPlanDto.Plan_Id);
            if (!isMember) return (false, "你必須先加入此計畫才能複製");

            var newPlanId = await _planMembersRepository.CopyPlanWithoutTopics(copyPlanDto.Plan_Id, copyPlanDto.User_Id);
            return newPlanId > 0 ? (true, "計畫複製成功") : (false, "複製失敗");
        }

    }
}
