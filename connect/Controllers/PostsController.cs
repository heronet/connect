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

    [HttpGet]
    public async Task<ActionResult> GetPosts()
    {
        var posts = await _dbContext.Posts
            .Include(p => p.User)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .Include(p => p.Photos)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
        var postDtos = posts.Select(p => PostToDto(p));
        return Ok(postDtos);
    }
    [Authorize]
    [HttpPost]
    public async Task<ActionResult> AddPost([FromForm] PostDto postDto)
    {
        if (postDto.Text.IsNullOrEmpty() && postDto.UploadPhotos.Count == 0)
            return BadRequest("Post cannot be blank");
        var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = await _userManager.FindByIdAsync(userId);
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

    [Authorize]
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
    [Authorize]
    [HttpPut("update/{postId}")]
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
    private PhotoDto PhotoToDto(Photo photo)
    {
        return new PhotoDto
        {
            Id = photo.Id,
            ImageUrl = photo.ImageUrl,
            PublicId = photo.PublicId
        };
    }
    private PostDto PostToDto(Post post)
    {
        return new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Text = post.Text,
            CreatedAt = post.CreatedAt,
            UserId = post.UserId,
            UserName = post.User.Name,
            LikesCount = post.Likes.Count,
            CommentsCount = post.Comments.Count,
            Photos = post.Photos.Select(p => PhotoToDto(p)).ToList()
        };
    }
}
