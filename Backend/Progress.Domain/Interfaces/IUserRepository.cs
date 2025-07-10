using Progress.Domain.Model;

namespace Progress.Domain.Interfaces
{
  public interface IUserRepository
  {
    User? GetByUsername(string username);
  }
}
