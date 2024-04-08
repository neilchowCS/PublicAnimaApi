using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using static AnimaApi.AuthenticationModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace AnimaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimaAuthenticationController : ControllerBase
    {
        private readonly IConfiguration _config;
        public AnimaAuthenticationController(IConfiguration config)
        {
            _config = config;
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login([FromBody] UserLogin userLogin)
        {
            Console.WriteLine(userLogin.Username + " " + userLogin.Password);
            if (User.Claims.Any())
            {
                Console.WriteLine(User.Claims.First().Value);
            }

            var user = Authenticate(userLogin);
            if (user.userId != null && user.role != null)
            {
                var token = GenerateToken(user.userId, user.role);
                return Ok(token);
            }

            return NotFound("user not found");
        }

        // To generate token
        private string GenerateToken(int? userId, UserRoles? role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim("userId", userId.ToString()!),
                new Claim("role", role.ToString()!)
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        //To authenticate user
        private (int? userId,UserRoles? role) Authenticate(UserLogin userLogin)
        {
            UserModel? output;
            var currentUser = UserConstants.Users.TryGetValue(userLogin.Username.ToLower(), out output);
                /*.FirstOrDefault(x => x.Username.ToLower() ==
                userLogin.Username.ToLower() && x.Password == userLogin.Password);*/
            if (currentUser && output.Password == userLogin.Password)
            {
                return (output.UserId, output.Role);
            }
            return (null, null);
        }

        //For admin Only
        [HttpPost]
        [Route("Admins")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminEndPoint([FromBody] string username)
        {
            var currentUser = GetCurrentUser(username);
            if (currentUser != null)
            {
                return Ok($"{username} is a(n) {currentUser.Role}");
            }
            return BadRequest("no such user");
        }
        private UserModel GetCurrentUser(string username)
        {
            /*
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaims = identity.Claims;
                return new UserModel
                {
                    Username = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value,
                    Role = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value
                };
            }
            return null;*/
            UserModel? userModel;
            AuthenticationModels.UserConstants.Users.TryGetValue(username, out userModel);
            return userModel;
        }
    }
}