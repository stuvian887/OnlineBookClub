using HtmlAgilityPack;
using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using OnlineBookClub.Repository;
using System.Text.Json;

public class BookService
{
    private readonly BookRepository _bookRepository;
    private readonly OnlineBookClubContext _context;

    public BookService(BookRepository bookRepository, OnlineBookClubContext context)
    {
        _bookRepository = bookRepository;
        _context = context;
    }

    public async Task<BookDTO?> GetBookInfoFromGoogleAsync(string isbn)
    {
        // 建立 HttpClient
        using var httpClient = new HttpClient();

        // 把 ISBN 組合成查詢網址
        var apiUrl = $"https://www.googleapis.com/books/v1/volumes?q=isbn:{isbn}";

        // 發送 GET 請求到 Google
        var response = await httpClient.GetAsync(apiUrl);

        // 如果請求失敗就返回 null
        if (!response.IsSuccessStatusCode) return null;

        // 讀取回傳的 JSON 字串
        var content = await response.Content.ReadAsStringAsync();

        // 用 System.Text.Json 解析 JSON
        var json = JsonDocument.Parse(content);
        if (!json.RootElement.TryGetProperty("items", out var items) || items.GetArrayLength() == 0)
        {
            return null; // 查不到資料，避免 KeyNotFoundException
        }

        // 拿出第一本書的資料
        var item = json.RootElement.GetProperty("items")[0];

        var info = item.GetProperty("volumeInfo");

        // 取得書名
        var title = info.GetProperty("title").GetString();

        // 若 description 存在就拿，否則設空字串
        var description = info.TryGetProperty("description", out var descProp) ? descProp.GetString() : "";

        // 拿 infoLink（Google 提供的書籍頁面）
        var infoLink = info.TryGetProperty("infoLink", out var linkProp) ? linkProp.GetString() : "";
        
        // 封面圖片（有些書可能沒有）
        string? imageUrl = null;
        if (info.TryGetProperty("imageLinks", out var imageLinks) &&
            imageLinks.TryGetProperty("thumbnail", out var thumb))
        {
            imageUrl = thumb.GetString();
        }

        // 回傳資料給前端
        return new BookDTO
        {
            BookName = title,
            Description = description,
            Link = infoLink,
            bookurl = imageUrl
        };
    }

    public async Task<BookDTO?> GetBookByPlanIdAsync(int planId, HttpRequest request)
    {
        var book = await _bookRepository.GetBookByPlanIdAsync(planId);
        if (book == null) return null;
        var url = "";
        var hostUrl = $"{request.Scheme}://{request.Host}";
        if (string.IsNullOrEmpty(book.bookpath)) book.bookpath = null;
        else if (book.bookpath.Contains("/book/"))
        {
            url = $"{hostUrl}{book.bookpath}";
        } else { url = book.bookpath; }
        var bookDto = new BookDTO
        {
            BookName = book.BookName,
            Description = book.Description,
            Link = book.Link,
            bookurl =url,
        };

        return bookDto;
    }

    public async Task<(Book?, string)> AddBookAsync(int planId, BookDTO bookDto)
    {
        var plan = await _context.BookPlan.FindAsync(planId);
        if (plan == null)
            return (null, "書籍新增失敗，找不到該書籍計畫");

        var savedFilePath = await SaveBookCoverAsync(bookDto.BookCover);
        if (savedFilePath == null) { savedFilePath= bookDto.bookurl; }
        var book = new Book
        {
            Plan_Id = planId,
            BookName = bookDto.BookName,
            Description = bookDto.Description,
            Link = bookDto.Link,
            bookpath = savedFilePath
        };

        await _bookRepository.AddBookAsync(planId, book);
        return (book, "書籍新增成功");
    }

    public async Task<(Book?, string)> UpdateBookAsync(int planId, BookDTO bookDto)
    {
        var plan = await _context.BookPlan.FindAsync(planId);
        if (plan == null)
            return (null, "書籍修改失敗，找不到該書籍計畫");

        var savedFilePath = await SaveBookCoverAsync(bookDto.BookCover);

        var book = new Book
        {
            BookName = bookDto.BookName,
            Description = bookDto.Description,
            Link = bookDto.Link,
            bookpath = savedFilePath // null 就不會更新
        };

        await _bookRepository.UpdateBookAsync(planId, book);
        return (book, "書籍修改成功");
    }

    private async Task<string?> SaveBookCoverAsync(IFormFile? file)
    {
        if (file == null) return null;

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Book", "images");

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, fileName);
        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/Book/images/{fileName}";
    }
    public async Task<BookDTO> GetBookInfoAsync(string url)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

        var html = await httpClient.GetStringAsync(url);

        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        string title = "", description = "", imageUrl = "";
      
        Console.WriteLine(doc.DocumentNode.OuterHtml); // 印出實際抓到的 HTML 原始碼

        // 判斷網站來源
        if (url.Contains("books.com.tw"))
        {
            // 博客來
            title = doc.DocumentNode.SelectSingleNode("//meta[@property='og:title']")?.GetAttributeValue("content", "").Trim();
            description = doc.DocumentNode
                .SelectSingleNode("//meta[@name='description']")
                ?.GetAttributeValue("content", "")
                .Trim();
            imageUrl = doc.DocumentNode
                .SelectSingleNode("//meta[@property='og:image']")
                ?.GetAttributeValue("content", "");
        }
        else if (url.Contains("eslite.com"))
        {
            title = doc.DocumentNode.SelectSingleNode("//meta[@property='og:title']")?.GetAttributeValue("content", "").Trim();
            description = doc.DocumentNode
                .SelectSingleNode("//meta[@name='description']")
                ?.GetAttributeValue("content", "")
                .Trim();
            imageUrl = doc.DocumentNode
                .SelectSingleNode("//meta[@property='og:image']")
                ?.GetAttributeValue("content", "");
        }
        else
        {
            // 其他網站使用通用 fallback
            title = doc.DocumentNode.SelectSingleNode("//title")?.InnerText?.Trim();
            description = doc.DocumentNode
                .SelectSingleNode("//meta[@name='description']")
                ?.GetAttributeValue("content", "")
                .Trim();
            imageUrl = doc.DocumentNode
                .SelectSingleNode("//meta[@property='og:image']")
                ?.GetAttributeValue("content", "");

            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                imageUrl = doc.DocumentNode
                    .SelectSingleNode("//img")
                    ?.GetAttributeValue("src", "");
            }
        }

        return new BookDTO
        {
            BookName = title ?? "（無書名）",
            Description = description ?? "（無簡介）",
            Link = url,
            bookurl = imageUrl ?? ""
        };
    }

}
