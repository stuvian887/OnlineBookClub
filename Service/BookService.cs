using Microsoft.EntityFrameworkCore;
using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using OnlineBookClub.Repository;

namespace OnlineBookClub.Service
{
    public class BookService
    {
        private readonly BookRepository _bookRepository;
        private readonly OnlineBookClubContext _context;
        public BookService(BookRepository bookRepository , OnlineBookClubContext context)
        {
            _bookRepository = bookRepository;
            _context = context;
        }
        public async Task<BookDTO?> GetBookByPlanIdAsync(int planId)
        {
            var book = await _bookRepository.GetBookByPlanIdAsync(planId);

            if (book == null) return null;

            return new BookDTO
            {
                BookName = book.BookName,
                Description = book.Description,
                Link = book.Link
            };
        }

        public async Task<(Book,string Message)> AddBookAsync(int planId, BookDTO bookDto)
        {
            var Plan = await _context.BookPlan.FindAsync(planId);
            if (Plan != null)
            {
                var book = new Book
                {
                    BookName = bookDto.BookName,
                    Description = bookDto.Description,
                    Link = bookDto.Link
                };

                await _bookRepository.AddBookAsync(planId, book);
                return (book , "書籍新增成功");
            }
            else
            {
                return (null , "書籍新增失敗，找不到該書籍");
            }
        }
    }
}
