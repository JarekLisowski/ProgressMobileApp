using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Progress.BusinessLogic;
using Progress.Domain.Api.Request;
using Progress.Domain.Api.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Progress.Api.Controllers
{
  [Route("api/auth")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    private readonly IConfiguration _config;
    private readonly AuthManager _authManager;
    private readonly IMapper _mapper;

    public AuthController(IConfiguration config, AuthManager authManager, IMapper mapper)
    {
      _config = config;
      _authManager = authManager;
      _mapper = mapper;
    }

    [HttpPost("login")]
    public LoginResponse Login([FromBody] LoginRequest login)
    {
      var user = _authManager.Authenticate(login.Username, login.Password);

      if (user != null)
      {
        var expireTime = DateTime.Now.AddHours(24);
        var token = GenerateToken(user, expireTime);
        var apiUser = _mapper.Map<Domain.Api.User>(user);
        apiUser.Token = token;
        apiUser.ExpirationDate = expireTime;
        return new LoginResponse
        {
          Data = new Domain.Api.Login
          {
            User = apiUser
          }          
        };
      }
      return new LoginResponse
      {
        IsError = true,
        Message = "Nieprawidłowy użytkownik lub hasło lub użytkownik nie jest oznaczony jako mobilny"
      };
      //return Unauthorized();
    }

    private string GenerateToken(Domain.Model.User user, DateTime expireTime)
    {
      var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
      var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
      var claims = new[]
      {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Login),
            };
      var token = new JwtSecurityToken(_config["Jwt:Issuer"],
          _config["Jwt:Audience"],
          claims,
          expires: expireTime,
          signingCredentials: credentials);

      return new JwtSecurityTokenHandler().WriteToken(token);
    }
  }
}
