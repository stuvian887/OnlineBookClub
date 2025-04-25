using Microsoft.Extensions.Hosting;
using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using OnlineBookClub.Repository;

public class ReplyService 
{
    private readonly ReplyRepository _repository;

    public ReplyService(ReplyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Reply> CreateReplyAsync(int userId, ReplyDTO dto)
    {
        string imgPath = "";
        if (dto.ReplyCover != null && dto.ReplyCover.Length > 0)
        {
            var uploadsFolder = Path.Combine("wwwroot", "Reply/imgs");
            Directory.CreateDirectory(uploadsFolder);
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ReplyCover.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.ReplyCover.CopyToAsync(stream);
            }

            imgPath = Path.Combine("Reply/imgs", fileName).Replace("\\", "/");
        }
        var reply = new Reply
        {
            Post_Id = dto.PostId,
            User_Id = userId,
            ReplyContent = dto.ReplyContent,
            ReplyImg = imgPath,
            ReplyTime = DateTime.UtcNow
        };

        return await _repository.CreateReplyAsync(reply);
    }

    public async Task<IEnumerable<ReplyDTO>> GetRepliesByPostIdAsync(int postId, HttpRequest request)
    {
        var reply = await _repository.GetRepliesByPostIdAsync(postId);
        var hostUrl = $"{request.Scheme}://{request.Host}"; // ex: https://localhost:7009

        return reply.Select(reply => new ReplyDTO
        {
            PostId = reply.Post_Id,
            ReplyContent = reply.ReplyContent,
            ReplyImg = string.IsNullOrEmpty(reply.ReplyImg) ? null : $"{hostUrl}{reply.ReplyImg}"
        }).ToList(); ;
    }

    public async Task<bool> UpdateReplyAsync(int replyId, int userId, ReplyDTO dto)
    {
        var reply = await _repository.GetReplyByIdAsync(replyId);
        if (reply == null || reply.User_Id != userId)
            return false;

        reply.ReplyContent = dto.ReplyContent;
        reply.ReplyImg = dto.ReplyImg ?? "";

        return await _repository.UpdateReplyAsync(reply);
    }

    public async Task<bool> DeleteReplyAsync(int replyId, int userId)
    {
        var reply = await _repository.GetReplyByIdAsync(replyId);
        if (reply == null || reply.User_Id != userId)
            return false;

        return await _repository.DeleteReplyAsync(replyId);
    }
}
