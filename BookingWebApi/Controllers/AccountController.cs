using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebApi.Dtos;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IUnitOfWork uwo;
        private readonly IConfiguration configuration;
        public AccountController(IUnitOfWork uwo, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.uwo = uwo;

        }
        // api/account/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(LoginReqDto loginReq)
        {
            if(await uwo.UserRepository.UserAlreadyExists(loginReq.UserName))
                return BadRequest("User already exists");
            uwo.UserRepository.Register(loginReq.UserName, loginReq.Password);
            await uwo.SaveAsync();
            return StatusCode(201);
        }
        // api/account/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginReqDto loginRq)
        {
            var user = await uwo.UserRepository.Authenticate(loginRq.UserName, loginRq.Password);
            if (user == null)
            {
                return Unauthorized();
            }
            else
            {
                LoginResDto loginRes = new LoginResDto();
                loginRes.UserName = user.Username;
                loginRes.Token = CreateJWT(user);
                return Ok(loginRes);
            }
        }
        private string CreateJWT(User user)
        {
            string secretKey = configuration.GetSection("AppSettings:Key").Value;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var claim = new Claim[] {
                new Claim(ClaimTypes.Name,user.Username),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())
            };

            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claim),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = signingCredentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}