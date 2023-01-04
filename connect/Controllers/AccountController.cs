using connect.Data.Dto;
using connect.Models;
using connect.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
            UserName = registerDto.Email.ToLower().Trim(),
            Email = registerDto.Email.ToLower().Trim(),
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
        var User = await _userManager.FindByEmailAsync(loginDto.Email.ToLower().Trim());

        // Return If User was not found
        if (User == null) return BadRequest("Invalid Email");

        var result = await _signInManager.CheckPasswordSignInAsync(User, password: loginDto.Password, false);
        if (result.Succeeded)
        {
            var roles = await _userManager.GetRolesAsync(User);
            return await UserToDto(User, roles.ToList());
        }

        return BadRequest("Invalid Password");
    }
    /// <summary>
    /// POST api/account/refresh
    /// </summary>
    /// <param name="UserAuthDto"></param>
    /// <returns><see cref="UserAuthDto" /></returns>
    [Authorize]
    [HttpPost("refresh")]
    public async Task<ActionResult<UserAuthDto>> RefreshToken(UserAuthDto userAuthDto)
    {

        var User = await _userManager.FindByIdAsync(userAuthDto.Id);

        // Return If User was not found
        if (User == null) return BadRequest("Invalid User");

        var roles = await _userManager.GetRolesAsync(User);
        return await UserToDto(User, roles.ToList());
    }

    /// <summary>
    /// Utility Method.
    /// Converts a WhotUser to an AuthUserDto
    /// </summary>
    /// <param name="User"></param>
    /// <returns><see cref="UserAuthDto" /></returns>
    private async Task<UserAuthDto> UserToDto(User User, List<string> roles)
    {
        return new UserAuthDto
        {
            Name = User.UserName,
            Token = await _tokenService.GenerateToken(User),
            Id = User.Id,
            Roles = roles
        };
    }
}