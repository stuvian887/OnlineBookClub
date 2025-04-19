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

        public BookService(BookRepository bookRepository)
        {
            _bookRepository = bookRepository;
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

        public async Task AddBookAsync(int planId, BookDTO bookDto)
        {
            string? savedFilePath = null;

            if (bookDto.BookCover != null)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(bookDto.BookCover.FileName)}";
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

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
                savedFilePath = $"/images/{fileName}";
            }

            var book = new Book
            {
                BookName = bookDto.BookName,
                Description = bookDto.Description,
                Link = bookDto.Link,
                 bookpath= savedFilePath  // 這個欄位要在 Book Model 裡加上
            };

            await _bookRepository.AddBookAsync(planId, book);
        }
    }
}
