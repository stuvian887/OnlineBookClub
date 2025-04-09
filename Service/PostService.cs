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
            var post = new Post
            {
                Plan_Id = dto.PlanId,
                User_Id = userId,
                Name = userName,
                Content = dto.Content,
                Img_Path = dto.ImgPath ?? "",
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
            post.Img_Path = dto.ImgPath ?? "";

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
