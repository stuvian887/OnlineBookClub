using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using OnlineBookClub.Repository;

namespace OnlineBookClub.Service
{
    public class BookService
    {
        private readonly BookRepository _bookRepository;

        public BookService(BookRepository bookRepository)
        {
            _bookRepository = bookRepository;
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

        public async Task AddBookAsync(int planId, BookDTO bookDto)
        {
            var book = new Book
            {
                BookName = bookDto.BookName,
                Description = bookDto.Description,
                Link = bookDto.Link
            };

            await _bookRepository.AddBookAsync(planId, book);
        }
    }
}
