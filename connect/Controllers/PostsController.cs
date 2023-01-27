using System.Security.Claims;
using connect.Data;
using connect.Data.Dto;
using connect.Models;
using connect.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace connect.Controllers;

[Authorize]
public class PostsController : BaseController
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<User> _userManager;
    private readonly PhotoService _photoService;
    public PostsController(ApplicationDbContext dbContext, UserManager<User> userManager, PhotoService photoService)
    {
        _photoService = photoService;
        _dbContext = dbContext;
        _userManager = userManager;
    }
    // Viewing posts does not require authentication.
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult> GetPosts(int skip = 0, int take = 20)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userManager.FindByIdAsync(userId);
        var posts = await _dbContext.Posts
            .Include(p => p.User)
            .ThenInclude(u => u.Avatar)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .Include(p => p.Photos)
            .OrderByDescending(p => p.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
        // If user is authenticated (user != null), check their likes
        var postDtos = posts.Select(p => PostToDto(p, user)).ToList();
        return Ok(postDtos);
    }
    // Viewing posts does not require authentication.
    [AllowAnonymous]
    [HttpGet("{postId}")]
    public async Task<ActionResult> GetPost(Guid postId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userManager.FindByIdAsync(userId);
        var post = await _dbContext.Posts
            .Include(p => p.User)
            .ThenInclude(u => u.Avatar)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .Include(p => p.Photos)
            .Where(p => p.Id == postId)
            .FirstOrDefaultAsync();
        // If user is authenticated (user != null), check their likes and comments
        if (post == null) return BadRequest("Post does not exist");
        var postDto = PostToDto(post, user);
        return Ok(postDto);
    }
    [HttpPost]
    public async Task<ActionResult> AddPost([FromForm] PostDto postDto)
    {
        if (postDto.Text.IsNullOrEmpty() && postDto.UploadPhotos.Count == 0)
            return BadRequest("Post cannot be blank");
        var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = await _userManager.Users
            .Include(u => u.Avatar)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return BadRequest("User does not exist");

        // Photo upload
        if (postDto.UploadPhotos.Count > 5)
            return BadRequest("Can't add more than 5 photos");
        var uploadedPhotos = new List<Photo>();
        foreach (var photo in postDto.UploadPhotos)
        {
            var photoResult = await _photoService.AddPhotoAsync(photo);
            if (photoResult.Error != null)
                return BadRequest(photoResult.Error.Message);
            var newPhoto = new Photo
            {
                ImageUrl = photoResult.SecureUrl.AbsoluteUri,
                PublicId = photoResult.PublicId
            };
            uploadedPhotos.Add(newPhoto);
        }

        var post = new Post
        {
            Title = postDto.Text?.Trim().Substring(0, Math.Min(40, postDto.Text.Trim().Length)) ?? "",
            Text = postDto.Text?.Trim() ?? "",
            User = user,
            Likes = new List<Like>(),
            Comments = new List<Comment>(),
            Photos = uploadedPhotos
        };
        _dbContext.Posts.Add(post);
        if (await _dbContext.SaveChangesAsync() > 0)
            return Ok(PostToDto(post));
        return BadRequest("Creating Post Failed");
    }

    [HttpDelete("delete/{postId}")]
    public async Task<ActionResult> DeletePost(Guid postId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return BadRequest("User does not exist");
        var post = await _dbContext.Posts
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .Include(p => p.Photos)
            .FirstOrDefaultAsync(p => p.Id == postId);
        if (post == null)
            return BadRequest("Post does not exist");
        if (post.UserId != userId)
            return Unauthorized("You cannot delete this post");

        // Delete photos
        foreach (var photo in post.Photos)
        {
            await _photoService.DeletePhotoAsync(photo.PublicId);
        }
        _dbContext.Posts.Remove(post);
        if (await _dbContext.SaveChangesAsync() > 0)
            return Ok(new { Message = $"Post {post.Id} deleted" });
        return BadRequest("Deleting Post Failed");
    }

    [HttpPut("update")]
    public async Task<ActionResult> UpdatePost(PostDto postDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return BadRequest("User does not exist");
        var post = await _dbContext.Posts
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .Include(p => p.Photos)
            .FirstOrDefaultAsync(p => p.Id == postDto.Id);
        if (post == null)
            return BadRequest("Post does not exist");
        if (post.UserId != userId)
            return Unauthorized("You cannot Edit this post");

        post.Title = postDto.Text?.Trim().Substring(0, Math.Min(40, postDto.Text.Trim().Length)) ?? "";
        post.Text = postDto.Text?.Trim() ?? "";
        _dbContext.Posts.Update(post);
        if (await _dbContext.SaveChangesAsync() > 0)
            return Ok(PostToDto(post));
        return BadRequest("Updating Post Failed");
    }

    [HttpPut("like")]
    public async Task<ActionResult> LikePost(PostDto postDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return BadRequest("User does not exist");
        var post = await _dbContext.Posts
            .Include(p => p.Likes)
            .FirstOrDefaultAsync(p => p.Id == postDto.Id);
        if (post == null)
            return BadRequest("Post does not exist");
        var like = post.Likes
            .Where(l => l.UserId == userId)
            .FirstOrDefault();
        // Like if not liked before
        if (like != null)
            return BadRequest("Cannot like twice");
        like = new Like
        {
            User = user,
            Post = post
        };
        post.Likes.Add(like);
        _dbContext.Posts.Update(post);
        if (await _dbContext.SaveChangesAsync() > 0)
            return Ok(new PostDto { Id = post.Id, PostLiked = true });
        return BadRequest("Like Post Failed");
    }

    [HttpPut("unlike")]
    public async Task<ActionResult> UnlikePost(PostDto postDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return BadRequest("User does not exist");
        var post = await _dbContext.Posts
            .Include(p => p.Likes)
            .FirstOrDefaultAsync(p => p.Id == postDto.Id);
        if (post == null)
            return BadRequest("Post does not exist");
        var like = post.Likes
            .Where(l => l.UserId == userId)
            .FirstOrDefault();
        if (like == null)
            return BadRequest("Cannot unlike post");
        post.Likes.Remove(like);
        _dbContext.Posts.Update(post);
        if (await _dbContext.SaveChangesAsync() > 0)
            return Ok(new PostDto { Id = post.Id, PostLiked = false });
        return BadRequest("Updating Post Failed");
    }
    [AllowAnonymous]
    [HttpGet("comments/{postId}")]
    public async Task<ActionResult> GetComments(Guid postId)
    {
        var comments = await _dbContext.Comments
            .Where(c => c.PostId == postId)
            .Include(c => c.User)
            .ThenInclude(u => u.Avatar)
            .OrderByDescending(c => c.Time)
            .ToListAsync();
        var commentDtos = comments.Select(c => CommentToDto(c));
        return Ok(commentDtos);
    }
    [HttpPost("comments")]
    public async Task<ActionResult> AddComment(CommentDto commentDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = await _userManager.Users
            .Include(u => u.Avatar)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return Unauthorized("You cannot comment");
        var post = await _dbContext.Posts
            .Include(p => p.Comments)
            .Where(p => p.Id == commentDto.PostId)
            .FirstOrDefaultAsync();
        if (post == null) return BadRequest("Post does not exist");
        var comment = new Comment
        {
            Text = commentDto.Text,
            User = user,
            Post = post
        };
        post.Comments.Add(comment);
        _dbContext.Posts.Update(post);
        if (await _dbContext.SaveChangesAsync() > 0)
            return Ok(CommentToDto(comment));
        return BadRequest("Failed to comment");
    }
    private PhotoDto PhotoToDto(Photo photo)
    {
        return new PhotoDto
        {
            Id = photo.Id,
            ImageUrl = photo.ImageUrl,
            PublicId = photo.PublicId
        };
    }
    private CommentDto CommentToDto(Comment comment)
    {
        return new CommentDto
        {
            Id = comment.Id,
            Text = comment.Text,
            Time = comment.Time,
            UserName = comment.User.Name,
            UserAvatarUrl = comment.User.Avatar?.ImageUrl,
            UserId = comment.UserId,
            PostId = comment.PostId
        };
    }
    private PostDto PostToDto(Post post, User user = null)
    {
        var liked = post.Likes.FirstOrDefault(l => l.UserId == user?.Id);
        return new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Text = post.Text,
            CreatedAt = post.CreatedAt,
            UserId = post.UserId,
            UserName = post.User.Name,
            UserAvatarUrl = post.User.Avatar?.ImageUrl,
            PostLiked = liked == null ? false : true,
            LikesCount = post.Likes.Count,
            CommentsCount = post.Comments.Count,
            Photos = post.Photos.Select(p => PhotoToDto(p)).ToList()
        };
    }
}
