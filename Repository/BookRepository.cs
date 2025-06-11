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
        public async Task UpdateBookAsync(int planId, Book book)
        {
            var existingBook = await _context.Book.FirstOrDefaultAsync(b => b.Plan_Id == planId);
            if (existingBook != null)
            {
                existingBook.BookName = book.BookName;
                existingBook.Description = book.Description;
                existingBook.Link = book.Link;

                if (!string.IsNullOrEmpty(book.bookpath))
                {
                    existingBook.bookpath = book.bookpath;
                }

                await _context.SaveChangesAsync();
            }
        }

    }
}
