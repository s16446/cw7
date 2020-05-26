using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.DAL;
using WebApplication1.DTOs.Requests;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{

    [ApiController]
    [Route("api/students/login")]
	public class LoginController : Controller
	{
 
        private IConfiguration  Configuration { get; set; }

        private IDbService _dbService;


        public LoginController(IDbService dbService, IConfiguration configuration)
        {
            _dbService = dbService;
            Configuration = configuration;
        }

      //  [HttpGet]
      //  [AllowAnonymous]
      //  public IActionResult PasswordHash()
      //  {
      //      Console.WriteLine(_dbService.PopulateWithData());
		    //return Ok();
      //   }
		
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login(LoginRequest request)
        {
            var claims = new[]
			{
			    new Claim(ClaimTypes.NameIdentifier, request.Login),
			    new Claim(ClaimTypes.Name, request.Login),
			    new Claim(ClaimTypes.Role, request.Role),
                new Claim(ClaimTypes.Hash, request.Password),
			};

        if (_dbService.CheckLogin(request.Login, request.Password))
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken
                (
                    issuer: "Gakko",
                    audience: "Students",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: creds
                );
            
                var refreshToken = Guid.NewGuid();
                var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
                _dbService.SaveRefreshToken(request.Login, refreshToken.ToString());

                return Ok(new {
                    accessToken,
                    refreshToken
                } );
            }
            else
            {
                return Unauthorized(request.Login + ": login or password is incorrect");
            } ;

        }

        [HttpPost("refresh-token/{token}/")]
        public IActionResult RefreshToken(string token)
        {
            string IndexNumber = _dbService.FindRefreshToken(token);
            if (IndexNumber != null)
            {
                var claims = new[]
			    {
			        new Claim(ClaimTypes.NameIdentifier, IndexNumber),
			        new Claim(ClaimTypes.Name, IndexNumber)
			    };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var newToken = new JwtSecurityToken
                (
                    issuer: "Gakko",
                    audience: "Students",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: creds
                );
            
                var refreshToken = Guid.NewGuid();
                var accessToken = new JwtSecurityTokenHandler().WriteToken(newToken);
                _dbService.SaveRefreshToken(IndexNumber, refreshToken.ToString());
              
                return Ok(new {
                      accessToken
                    , refreshToken
                } );
                
            }
            else
                return Unauthorized("Invalid token");

        }

	}
}
