using AspNetCoreWebAPI.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AspNetCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        /*       
        // GET: api/<AuthController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<AuthController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AuthController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<AuthController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AuthController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        */

        UserRepository user_repo;
        public AuthController(UserRepository user_repo)
        {
            this.user_repo = user_repo;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginUser loginData)
        {
            if (user_repo.TryLogin(loginData) is User user)
            {
                await Authenticate(loginData.Username); // аутентификация

                return Ok(user);
            }
            else return BadRequest("Wrong Username or/and Password");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterUser registerData)
        {
            try
            {
                User user = user_repo.AddUser(registerData);

                await Authenticate(registerData.Username); // аутентификация

                return Ok(user);
            } 
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task Authenticate(string userName)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
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
}
