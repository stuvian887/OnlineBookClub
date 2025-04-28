using Microsoft.Extensions.Hosting;
using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using OnlineBookClub.Repository;

public class ReplyService 
{
    private readonly ReplyRepository _repository;
    private readonly MembersRepository _membersRepository;
    public ReplyService(ReplyRepository repository, MembersRepository membersRepository)
    {
        _repository = repository;
        _membersRepository = membersRepository;
    }

    public async Task<Reply> CreateReplyAsync(int userId, CreateReply dto)
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
        var data = _membersRepository.getbyid(userId);
        var reply = new Reply
        {
            Post_Id = dto.PostId,
            User_Id = userId,
            ReplyContent = dto.ReplyContent,
            ReplyImg = imgPath,
            ReplyTime = DateTime.UtcNow,
        };

        return await _repository.CreateReplyAsync(reply);
    }

    public async Task<IEnumerable<ReplyDTO>> GetRepliesByPostIdAsync(int replyId, HttpRequest request)
    {
        var reply = await _repository.GetRepliesByPostIdAsync(replyId);
        var hostUrl = $"{request.Scheme}://{request.Host}"; // ex: https://localhost:7009
        var replys= new List<ReplyDTO>();
        foreach (var reply1 in reply)
        {
            var member = _membersRepository.getbyid(reply1.User_Id); // 每一篇都去找作者

            replys.Add(new ReplyDTO
            {
                ReplyId = reply1.Reply_Id,
                PostId = reply1.Post_Id,
                ReplyContent = reply1.ReplyContent,
                CreateTime = reply1.ReplyTime,
                ImgPath = string.IsNullOrEmpty(reply1.ReplyImg) ? null : $"{hostUrl}{reply1.ReplyImg}",
                MemberPath = member?.ProfilePictureUrl,  // 作者的大頭貼
                Name = member?.UserName                  // 作者的名字
            });
        }

        return replys;
    }

    public async Task<bool> UpdateReplyAsync(int replyId, int userId, CreateReply dto)
    {
        var reply = await _repository.GetReplyByIdAsync(replyId);
        if (reply == null || reply.User_Id != userId)
            return false;

        if (dto.ReplyCover != null && dto.ReplyCover.Length > 0)
        {
            var uploadsFolder = Path.Combine("wwwroot", "Post/imgs");
            Directory.CreateDirectory(uploadsFolder); // 確保資料夾存在

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ReplyCover.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.ReplyCover.CopyToAsync(stream);
            }

            reply.ReplyImg = Path.Combine("Post/imgs", uniqueFileName).Replace("\\", "/");
        }
        else if (!string.IsNullOrEmpty(reply.ReplyImg))
        {
            // 若未上傳圖片但有指定路徑（可能是保留原圖），則保留它
            reply.ReplyImg = reply.ReplyImg;
        }

        return await _repository.UpdateReplyAsync(reply);
    }

    public async Task<bool> DeleteReplyAsync(int replyId, int userId)
    {
        var reply = await _repository.GetReplyByIdAsync(replyId);
        if (reply == null || reply.User_Id != userId)
            return false;

        return await _repository.DeleteReplyAsync(replyId);
    }
    public async Task<ReplyDTO?> GetReplyByIdAsync(string email, int ReplyID)
    {
        var Reply = await _repository.GeReplyByIdWithUserAsync(ReplyID);
        if (Reply == null) return null;

        return new ReplyDTO
        {
            
            PostId = Reply.Post_Id,
            ReplyId= Reply.Reply_Id,
            ReplyContent = Reply.ReplyContent,
            ImgPath = Reply.ReplyImg,
            CreateTime = Reply.ReplyTime,
            MemberPath = Reply.User?.ProfilePictureUrl, // 改成拿發文者的資料
            Name = Reply.User?.UserName
        };
    }



    public async Task<IEnumerable<ReplyDTO>> GetReplyByUserIdAsync(int userId, string email)
    {
        var Replies = await _repository.GetReplyByUserIdAsync(userId);
        var data =_membersRepository.getbyid(userId);
        return Replies.Select(Reply => new ReplyDTO
        {
            ReplyId = Reply.Reply_Id,
            PostId = Reply.Post_Id,
            ReplyContent = Reply.ReplyContent,
            ImgPath = Reply.ReplyImg,
            CreateTime = Reply.ReplyTime,
            MemberPath = data.ProfilePictureUrl,
            Name = data.UserName
        }).ToList();
    }
}
