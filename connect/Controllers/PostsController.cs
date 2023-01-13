using System.Security.Claims;
using connect.Data;
using connect.Data.Dto;
using connect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace connect.Controllers;

public class PostsController : BaseController
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<User> _userManager;
    public PostsController(ApplicationDbContext dbContext, UserManager<User> userManager)
    {
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
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
        var postDtos = posts.Select(p => PostToDto(p));
        return Ok(postDtos);
    }
    [Authorize]
    [HttpPost]
    public async Task<ActionResult> AddPost(PostDto postDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return BadRequest("User does not exist");
        var post = new Post
        {
            Text = postDto.Text,
            User = user
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
        _dbContext.Posts.Remove(post);
        if (await _dbContext.SaveChangesAsync() > 0)
            return Ok(new { Message = $"Post {post.Id} deleted" });
        return BadRequest("Deleting Post Failed");
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
            LikesCount = post.Likes?.Count ?? 0,
            CommentsCount = post.Comments?.Count ?? 0
        };
    }
}
