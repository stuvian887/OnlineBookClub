
using Microsoft.EntityFrameworkCore;
using Azure.Core;
using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using OnlineBookClub.Repository;
using static System.Reflection.Metadata.BlobBuilder;

namespace OnlineBookClub.Service
{
    public class BookService
    {
        private readonly BookRepository _bookRepository;
        private readonly OnlineBookClubContext _context;
        public BookService(BookRepository bookRepository, OnlineBookClubContext context)
        {
            _bookRepository = bookRepository;
            _context = context;
        }
        public async Task<BookDTO?> GetBookByPlanIdAsync(int planId, HttpRequest request)
        {
            var book = await _bookRepository.GetBookByPlanIdAsync(planId);
            var hostUrl = $"{request.Scheme}://{request.Host}"; // ex: https://localhost:7009
            var bookDto = new BookDTO
            {
                BookName = book.BookName,
                Description = book.Description,
                Link = book.Link,
                bookurl = string.IsNullOrEmpty(book.bookpath) ? null : $"{hostUrl}{book.bookpath}"
            };

            return bookDto;
        }

        public async Task<(Book, string Message)> AddBookAsync(int planId, BookDTO bookDto)
        {
            var Plan = await _context.BookPlan.FindAsync(planId);
            if (Plan != null)
            {
                string? savedFilePath = null;
                if (bookDto.BookCover != null)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(bookDto.BookCover.FileName)}";
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Book/images");

                    // ✅ 檢查資料夾是否存在，不存在就建立
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    var filePath = Path.Combine(folderPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await bookDto.BookCover.CopyToAsync(stream);
                    }

                    // 儲存圖片的相對網址
                    savedFilePath = $"/Book/images/{fileName}";
                }
                var book = new Book
                {
                    BookName = bookDto.BookName,
                    Description = bookDto.Description,
                    Link = bookDto.Link
                };
                await _bookRepository.AddBookAsync(planId, book);
                return (book, "書籍新增成功");
            }
            else
            {
                return (null, "書籍新增失敗，找不到該書籍");
            }
        }
    }
}
