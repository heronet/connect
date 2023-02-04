using connect.Data.Dto;
using connect.Models;
using connect.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace connect.Controllers;

public class AccountController : BaseController
{
    private readonly UserManager<User> _userManager;
    private readonly TokenService _tokenService;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    public AccountController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        RoleManager<IdentityRole> roleManager,
        TokenService tokenService
    )
    {
        _roleManager = roleManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _userManager = userManager;
    }
    /// <summary>
    /// POST api/account/register
    /// </summary>
    /// <param name="registerDto"></param>
    /// <returns><see cref="UserAuthDto" /></returns>
    [HttpPost("register")]
    public async Task<ActionResult<UserAuthDto>> RegisterUser(RegisterDto registerDto)
    {
        var User = new User
        {
            UserName = Guid.NewGuid().ToString(),
            Email = registerDto.Email.ToLower().Trim(),
            Name = registerDto.Name.Trim(),
        };
        var result = await _userManager.CreateAsync(User, password: registerDto.Password);
        if (!result.Succeeded) return BadRequest(result);

        var addToRoleResult = await _userManager.AddToRoleAsync(User, "Member");
        if (addToRoleResult.Succeeded)
        {
            var roles = await _userManager.GetRolesAsync(User);
            return await UserToDto(User, roles.ToList());
        }
        return BadRequest("Can't add User");
    }
    /// <summary>
    /// POST api/account/login
    /// </summary>
    /// <param name="loginDTO"></param>
    /// <returns><see cref="UserAuthDto" /></returns>
    [HttpPost("login")]
    public async Task<ActionResult<UserAuthDto>> LoginUser(LoginDto loginDto)
    {
        var email = loginDto.Email.ToLower().Trim();
        var user = await _userManager.Users
            .Include(u => u.Avatar)
            .Where(u => u.Email == email)
            .FirstOrDefaultAsync();
        // Return If User was not found
        if (user == null) return BadRequest("Invalid Email");
        var result = await _signInManager.CheckPasswordSignInAsync(user, password: loginDto.Password, false);
        if (result.Succeeded)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return await UserToDto(user, roles.ToList());
        }
        return BadRequest("Invalid Password");
    }

    /// <summary>
    /// Utility Method.
    /// Converts a WhotUser to an AuthUserDto
    /// </summary>
    /// <param name="User"></param>
    /// <returns><see cref="UserAuthDto" /></returns>
    private async Task<UserAuthDto> UserToDto(User user, List<string> roles)
    {
        return new UserAuthDto
        {
            Email = user.Email,
            Token = await _tokenService.GenerateToken(user),
            Id = user.Id,
            Roles = roles,
            UserAvatarUrl = user.Avatar?.ImageUrl
        };
    }
}