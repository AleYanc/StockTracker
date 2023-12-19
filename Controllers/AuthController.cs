using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StockTracker.DTO.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StockTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration, IMapper mapper) : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager = userManager;
        private readonly IConfiguration configuration = configuration;
        private readonly IMapper mapper = mapper;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDTO userDTO)
        {
            IdentityUser userExists = await userManager.FindByEmailAsync(userDTO.Email);
            if(userExists != null) return BadRequest("The email already exists.");

            IdentityUser user = mapper.Map<IdentityUser>(userDTO);

            IdentityResult result = await userManager.CreateAsync(user, userDTO.Password);

            if(!result.Succeeded) return BadRequest($"Failed to register {userDTO.Email}");

            return Ok("User created successfully. You can now log-in.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> GetToken([FromForm] LoginDTO loginCredentials)
        {
            IdentityUser user = await userManager.FindByEmailAsync(loginCredentials.Email);
            if(user is null || !await userManager.CheckPasswordAsync(user, loginCredentials.Password)) return Unauthorized("There was a problem with your credentials.");

            IList<string> roles = await userManager.GetRolesAsync(user);

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Sid, user.Id),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            foreach(string role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            string jwt = GenerateJwt(claims);

            return Ok(new { Token = jwt });
        }

        private string GenerateJwt(List<Claim> claims)
        {
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256Signature);
            JwtSecurityToken tokenDescriptor = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            string jwt = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return jwt;
        }
    }
}
