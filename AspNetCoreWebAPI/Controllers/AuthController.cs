using AspNetCoreWebAPI.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AspNetCoreWebAPI.Repositories;

namespace AspNetCoreWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    public IUserRepository UserRepository;

    public AuthController(IUserRepository userRepository)
    {
        this.UserRepository = userRepository;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]UserDTO dto)
    {
        if (UserRepository.FindByDTO(dto) is User user)
        {
            // аутентификация - по сути запись в куки,
            // что это пользователь с таким то юзернеймом
            await Authenticate(dto.Username);

            return Ok(user);
        }
        else return BadRequest("Wrong Username or/and Password");
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody]UserDTO dto)
    {
        try
        {
            User user = UserRepository.AddUser(dto);

            await Authenticate(user.Username); // аутентификация

            return Ok(user);
        } 
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private async Task Authenticate(string username)
    {
        // создаем список наших клеймов (инфы о пользователе)
        var claims = new List<Claim>
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, username)
        };

        // создаем объект ClaimsIdentity
        ClaimsIdentity id = new ClaimsIdentity(
            claims, "ApplicationCookie", 
            ClaimsIdentity.DefaultNameClaimType, 
            ClaimsIdentity.DefaultRoleClaimType
        );

        // установка аутентификационных куки клиенту - они будут HttpOnly (что указано в Program.cs)
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok("Logged Out");
    }
}
