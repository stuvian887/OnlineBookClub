using Microsoft.Extensions.Hosting;
using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using OnlineBookClub.Repository;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OnlineBookClub.Service
{
    public class PostService 
    {
        private readonly PostRepository _repository;
        private readonly MembersRepository _membersRepository;
        public PostService(PostRepository repository, MembersRepository  membersRepository )
        {
            _repository = repository;
            _membersRepository = membersRepository;
        }

        public async Task<Post> CreatePostAsync(string email,int userId, string userName, CreatePost dto)
        {
            string imgPath = "";

            if (dto.PostCover != null && dto.PostCover.Length > 0)
            {
                var uploadsFolder = Path.Combine("wwwroot", "Post/imgs");
                Directory.CreateDirectory(uploadsFolder);
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.PostCover.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.PostCover.CopyToAsync(stream);
                }

                imgPath = Path.Combine("Post/imgs", fileName).Replace("\\", "/");
            }

            var post = new Post
            {
                Plan_Id = dto.PlanId,
                User_Id = userId,
                Name=userName,
                Content = dto.Content,
                Img_Path = imgPath,
                CreateTime = DateTime.UtcNow
            };

            return await _repository.CreatePostAsync(post);
            
            

        }


        public async Task<IEnumerable<PostDTO>> GetPostsByPlanIdAsync(string email, int planId, HttpRequest request)
        {
            var posts = await _repository.GetPostsByPlanIdAsync(planId);
            var hostUrl = $"{request.Scheme}://{request.Host}"; // ex: https://localhost:7009

            var postDtos = new List<PostDTO>();

            foreach (var post in posts)
            {
                var member =  _membersRepository.getbyid(post.User_Id); // 每一篇都去找作者

                postDtos.Add(new PostDTO
                {
                    PostId = post.Post_Id,
                    PlanId = post.Plan_Id,
                    Content = post.Content,
                    CreateTime = post.CreateTime,
                    ImgPath = string.IsNullOrEmpty(post.Img_Path) ? null : $"{hostUrl}{post.Img_Path}",
                    MemberPath = member?.ProfilePictureUrl,  // 作者的大頭貼
                    Name = member?.UserName                  // 作者的名字
                });
            }

            return postDtos;
        }


        public async Task<bool> UpdatePostAsync(int postId, int userId, CreatePost dto)
        {
            var post = await _repository.GetPostByIdAsync(postId);
            if (post == null || post.User_Id != userId)
                return false;

            post.Content = dto.Content;

            // 如果有上傳圖片，儲存圖片並更新路徑
            if (dto.PostCover != null && dto.PostCover.Length > 0)
            {   
                var uploadsFolder = Path.Combine("wwwroot", "Post/imgs");
                Directory.CreateDirectory(uploadsFolder); // 確保資料夾存在

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.PostCover.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.PostCover.CopyToAsync(stream);
                }

                post.Img_Path = Path.Combine("Post/imgs", uniqueFileName).Replace("\\", "/");
            }
            else if (!string.IsNullOrEmpty(post.Img_Path))
            {
                // 若未上傳圖片但有指定路徑（可能是保留原圖），則保留它
                post.Img_Path = post.Img_Path;
            }

            return await _repository.UpdatePostAsync(post);
        }


        public async Task<bool> DeletePostAsync(int postId, int userId)
        {
            var post = await _repository.GetPostByIdAsync(postId);
            if (post == null || post.User_Id != userId)
                return false;

            return await _repository.DeletePostAsync(postId);
        }

        public async Task<PostDTO?> GetPostByIdAsync(string email, int postId)
        {
            var post = await _repository.GetPostByIdWithUserAsync(postId);
            if (post == null) return null;

            return new PostDTO
            {
                PlanId = post.Plan_Id,
                PostId = post.Post_Id,
                Content = post.Content,
                ImgPath = post.Img_Path,
                CreateTime = post.CreateTime,
                MemberPath = post.User?.ProfilePictureUrl, // 改成拿發文者的資料
                Name = post.User?.UserName
            };
        }



        public async Task<IEnumerable<PostDTO>> GetPostsByUserIdAsync(int userId,string email)
        {
            var posts = await _repository.GetPostsByUserIdAsync(userId);
            var data = _membersRepository.getbyid(userId);
            return posts.Select(post => new PostDTO
            {
                PlanId = post.Plan_Id,
                PostId = post.Post_Id,
                Content = post.Content,
                ImgPath = post.Img_Path,
                CreateTime = post.CreateTime,
                MemberPath = data.ProfilePictureUrl,
                Name = data.UserName
            }).ToList();
        }

    }
}
