using HtmlAgilityPack;
using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using OnlineBookClub.Repository;
using System.Runtime.CompilerServices;

namespace OnlineBookClub.Service
{
    public class ChapterService
    {
        private readonly ChapterRepository _chapterRepository;
        public ChapterService(ChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }
        public async Task<(ChapterDTO , string Message)> CreateChapter(int UserId, int Plan_Id, ChapterDTO InsertData)
        {
            return await _chapterRepository.CreateChapterAsync(UserId, Plan_Id, InsertData);
        }
        public async Task<IEnumerable<ChapterDTO>> GetChapter(int Plan_Id)
        {
            var result = await _chapterRepository.GetChapterByPlanAsync(Plan_Id);
            return result;
        }
        public async Task<Chapter> GetSingleChapter(int Chapter_Id)
        {
            var result = await _chapterRepository.GetSingleChapter(Chapter_Id);
            return result;
        }
        public async Task<(ChapterDTO, string Message)> UpdateChapter(int UserId, int Chapter_Id, ChapterDTO UpdateData)
        {
            return await _chapterRepository.UpdateChapterAsync(UserId, Chapter_Id, UpdateData); 
        }
        public async Task<string> DeleteChapter(int UserId, int Chapter_Id)
        {
            return await _chapterRepository.DeleteChapterAsync(UserId, Chapter_Id);
        }
        public async Task<ChapterDTO> CopyChapters(int original_planId, int source_plan_Id , int Chapter_Id)
        {
            return await _chapterRepository.CopyChapter(original_planId ,source_plan_Id ,Chapter_Id);
        }
    }
}
