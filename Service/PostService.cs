using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using OnlineBookClub.Repository;

namespace OnlineBookClub.Service
{
    public class PostService 
    {
        private readonly PostRepository _repository;

        public PostService(PostRepository repository)
        {
            _repository = repository;
        }

        public async Task<Post> CreatePostAsync(int userId, string userName, PostDTO dto)
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
                Name = userName,
                Content = dto.Content,
                Img_Path = imgPath,
                CreateTime = DateTime.UtcNow
            };

            return await _repository.CreatePostAsync(post);
        }


        public async Task<IEnumerable<Post>> GetPostsByPlanIdAsync(int planId)
        {
            return await _repository.GetPostsByPlanIdAsync(planId);
        }

        public async Task<bool> UpdatePostAsync(int postId, int userId, PostDTO dto)
        {
            var post = await _repository.GetPostByIdAsync(postId);
            if (post == null || post.User_Id != userId)
                return false;

            post.Content = dto.Content;

            // 如果有上傳圖片，儲存圖片並更新路徑
            if (dto.PostCover != null && dto.PostCover.Length > 0)
            {   
                var uploadsFolder = Path.Combine("wwwroot", "postImages");
                Directory.CreateDirectory(uploadsFolder); // 確保資料夾存在

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.PostCover.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.PostCover.CopyToAsync(stream);
                }

                post.Img_Path = Path.Combine("postImages", uniqueFileName).Replace("\\", "/");
            }
            else if (!string.IsNullOrEmpty(dto.ImgPath))
            {
                // 若未上傳圖片但有指定路徑（可能是保留原圖），則保留它
                post.Img_Path = dto.ImgPath;
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
    }
}
