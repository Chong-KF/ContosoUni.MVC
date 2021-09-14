using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using ContosoUni.Areas.Identity.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using ContosoUni.Areas.JWT.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace ContosoUni.Areas.JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly SignInManager<MyIdentityUser> _signInManager;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(
            SignInManager<MyIdentityUser> signInManager,
            ILogger<AuthenticationController> logger,
            UserManager<MyIdentityUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        // POST api/<AccountController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserRequest obj)
        {
            if (ModelState.IsValid)
            {
                MyIdentityUser user = await _userManager.FindByNameAsync(obj.Email);
                if (user != null)
                {
                    //var result = await _signInManager.PasswordSignInAsync(user, obj.Password, false, lockoutOnFailure: false);
                    var result = await _signInManager.CheckPasswordSignInAsync(user, obj.Password, false);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"Issue JWT token to {user.Email}.");

                        //create JWT token
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var securityKey = Encoding.UTF8.GetBytes(MyJwt.Key);
                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Issuer = MyJwt.Issuer,
                            Audience = MyJwt.Audience,
                            Subject = new ClaimsIdentity(new Claim[]
                            {
                                new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // this guarantees the token is unique
                                new Claim("displayname", user.DisplayName)
                            }),
                            Expires = DateTime.UtcNow.AddHours(1),
                            SigningCredentials = new SigningCredentials(
                                new SymmetricSecurityKey(securityKey),
                                    SecurityAlgorithms.HmacSha256Signature)
                        };
                        //add user roles
                        IList<string> roles = await _userManager.GetRolesAsync(user);
                        foreach (var role in roles)
                        {
                            tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, role));
                        }
                        return Ok(tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor)));
                    }
                }
            }
            return StatusCode(StatusCodes.Status401Unauthorized);
        }
    }
}