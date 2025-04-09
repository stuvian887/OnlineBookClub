using Microsoft.EntityFrameworkCore;
using OnlineBookClub.Models;

namespace OnlineBookClub.Repository
{
    public class BookRepository
    {
        private readonly OnlineBookClubContext _context;

        public BookRepository(OnlineBookClubContext context)
        {
            _context = context;
        }
        public async Task<Book?> GetBookByPlanIdAsync(int planId)
        {
            return await _context.Book
                .FirstOrDefaultAsync(b => b.Plan_Id == planId);
        }
        public async Task AddBookAsync(int planId, Book book)
        {
            book.Plan_Id = planId;
            _context.Book.Add(book);
            await _context.SaveChangesAsync();
        }
    }
}
