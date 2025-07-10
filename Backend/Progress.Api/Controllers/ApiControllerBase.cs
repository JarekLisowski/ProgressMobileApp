using Microsoft.AspNetCore.Mvc;
using Progress.Domain.Model;
using Progress.Infrastructure.Database.Repository;
using System.Security.Claims;

namespace Progress.Api.Controllers
{
  public class ApiControllerBase : ControllerBase
  {
    protected IServiceProvider ServiceProvider { get; }
    UserRepository? _userRepository;
    User? _user;

    public ApiControllerBase(IServiceProvider serviceProvider) 
    {
      ServiceProvider = serviceProvider;
    }

    private int? _userId;
    protected int? GetUserId()
    {
      if (_userId != null)
        return _userId;

      var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
      if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
      {
        _userId = userId;
        return userId;
      }

      return null;
    }

    private UserRepository GetUserRepository()
    {
      if ( _userRepository == null)
        _userRepository = ServiceProvider.GetRequiredService<UserRepository>();
      return _userRepository;
    }

    protected User? GetUser()
    {
      if (_user != null)
      {
        return _user;
      }
      var userId = GetUserId();
      if (userId != null)
      {
        var user = GetUserRepository().GetUser(userId.Value);
        if (user != null)
        {
          _user = user;
          return user;
        }
      }
      return null;
    }
  }
}
