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
        var reply = new Reply
        {
            Post_Id = dto.PostId,
            User_Id = userId,
            ReplyContent = dto.ReplyContent,
            ReplyImg = dto.ReplyImg ?? "",
            ReplyTime = DateTime.UtcNow
        };

        return await _repository.CreateReplyAsync(reply);
    }

    public async Task<IEnumerable<Reply>> GetRepliesByPostIdAsync(int postId)
    {
        return await _repository.GetRepliesByPostIdAsync(postId);
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
