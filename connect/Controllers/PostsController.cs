using connect.Data;
using connect.Data.Dto;
using connect.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace connect.Controllers;

public class PostsController : BaseController
{
    private readonly ApplicationDbContext _dbContext;
    public PostsController(ApplicationDbContext dbContext) => _dbContext = dbContext;

    [HttpGet]
    public async Task<ActionResult> GetPosts()
    {
        var posts = await _dbContext.Posts
            .Include(p => p.User)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => PostToDto(p))
            .ToListAsync();
        return Ok(posts);
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
            LikesCount = post.Likes.Count
        };
    }
}
