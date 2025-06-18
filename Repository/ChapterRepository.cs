using Microsoft.EntityFrameworkCore;
using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using System.Numerics;

namespace OnlineBookClub.Repository
{
    public class ChapterRepository
    {
        private readonly OnlineBookClubContext _context;
        private readonly PlanMemberRepository _planMemberRepository;
        public ChapterRepository(OnlineBookClubContext context, PlanMemberRepository planMemberRepository)
        {
            _context = context;
            _planMemberRepository = planMemberRepository;
        }

        public async Task<string> CreateChapterAsync(int UserId, int PlanId, ChapterDTO InsertData)
        {
            
            BookPlan FindPlan = await _context.BookPlan.Include(bp => bp.Chapters).SingleOrDefaultAsync(p => p.Plan_Id == PlanId);
            if (FindPlan == null)
            {
                return ("錯誤，找不到該計畫");
            }
            var Role = await _planMemberRepository.GetUserRoleAsync(UserId, PlanId);
            if (Role != "組長")
            {
                return ("錯誤，你不是組長");
            }
            
            var AllChapter = await _context.Chapter.Where(l => l.Plan_Id == PlanId).ToListAsync();
            
            foreach (var chapters in AllChapter)
            {
                if (InsertData.Chapter_Index == chapters.Chapter_Index) return ("錯誤，此章節編號已經存在");
            }
            //不可超過5項
            int ChapterCount = await _context.Chapter.Where(l => l.Plan_Id == PlanId).CountAsync();
            if (ChapterCount >= 5) { return "錯誤，單一計畫的章節不可超過五個"; };
            var Chapter = new Chapter
            {
                Chapter_Index = InsertData.Chapter_Index,
                Chapter_Name = InsertData.Chapter_Name,
                Plan_Id = PlanId,
            };
            _context.Chapter.Add(Chapter);
            await _context.SaveChangesAsync();
            return "Success";
        }
        public async Task<IEnumerable<ChapterDTO>> GetChapterByPlanAsync(int PlanId)
        {
            var chap = (from c in _context.Chapter
                        where c.Plan_Id == PlanId
                        select new ChapterDTO
                        {
                            Chapter_Id = c.Chapter_Id,
                            Chapter_Index = c.Chapter_Index,
                            Chapter_Name = c.Chapter_Name,
                            Plan_Id = c.Plan_Id
                        }).ToListAsync();
            return await chap;
        }
        public async Task<(ChapterDTO , string Message)> UpdateChapterAsync(int UserId, int Chapter_Id, ChapterDTO UpdateData)
        {
            Chapter FindChapter = await _context.Chapter.Where(c => c.Chapter_Id == Chapter_Id).FirstOrDefaultAsync();
            if(FindChapter == null)
            {
                return (null, "找不到該章節");
            }
            BookPlan FindPlan = await _context.BookPlan.Include(bp => bp.Chapters).SingleOrDefaultAsync(p => p.Plan_Id == FindChapter.Plan_Id);
            if (FindPlan == null)
            {
                return (null, "錯誤，找不到該計畫");
            }
            var Role = await _planMemberRepository.GetUserRoleAsync(UserId, FindChapter.Plan_Id);
            if (Role != "組長")
            {
                return (null , "錯誤，你不是組長");
            }

            var AllChapter = await _context.Chapter.Where(l => l.Plan_Id == FindChapter.Plan_Id).ToListAsync();
            foreach (var chapters in AllChapter)
            {
                if (UpdateData.Chapter_Index == chapters.Chapter_Index) return (null , "錯誤，此章節編號已經存在");
            }
            FindChapter.Chapter_Index = UpdateData.Chapter_Index;
            FindChapter.Chapter_Name = UpdateData.Chapter_Name;
            var resultDto = new ChapterDTO
            {
                Chapter_Id = FindChapter.Chapter_Id,
                Chapter_Index = FindChapter.Chapter_Index,
                Chapter_Name = FindChapter.Chapter_Name,
                Plan_Id = FindChapter.Plan_Id,
            };
            await _context.SaveChangesAsync();
            return (null, "章節修改成功");
        }
        public async Task<string> DeleteChapterAsync(int UserId ,int ChapterId)
        {
            var Chapter = await _context.Chapter
                .Where(c => c.Chapter_Id == ChapterId)
                .FirstOrDefaultAsync();
            if (Chapter == null)
            {
                return ("找不到章節");
            }
            var Role = await _planMemberRepository.GetUserRoleAsync(UserId, Chapter.Plan_Id);
            if (Role != "組長")
            {
                return ("錯誤，你不是組長");
            }
            var learns = await _context.Learn.Where(l => l.Chapter_Id == ChapterId).ToListAsync();
            _context.Learn.RemoveRange(learns);
            _context.Remove(Chapter);
            await _context.SaveChangesAsync();
            return ("刪除成功");
        }
    }
}
