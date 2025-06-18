using HtmlAgilityPack;
using OnlineBookClub.DTO;
using OnlineBookClub.Repository;

namespace OnlineBookClub.Service
{
    public class ChapterService
    {
        private readonly ChapterRepository _chapterRepository;
        public ChapterService(ChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }
        public async Task<string> CreateChapter(int UserId, int Plan_Id, ChapterDTO InsertData)
        {
            string result = await _chapterRepository.CreateChapterAsync(UserId, Plan_Id, InsertData);
            return result;
        }
        public async Task<IEnumerable<ChapterDTO>> GetChapter(int Plan_Id)
        {
            var result = await _chapterRepository.GetChapterByPlanAsync(Plan_Id);
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
    }
}
