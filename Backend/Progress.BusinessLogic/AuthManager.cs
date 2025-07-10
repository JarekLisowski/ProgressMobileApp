using Progress.Domain.Interfaces;
using Progress.Domain.Model;

namespace Progress.BusinessLogic
{
  public class AuthManager
  {
    private readonly IUserRepository _userRepository;

    public AuthManager(IUserRepository userRepository)
    {
      _userRepository = userRepository;
    }

    public User? Authenticate(string? username, string? password)
    {
      if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        return null;

      var user = _userRepository.GetByUsername(username);

      if (user == null || !user.Mobile || !CheckPassword(password, user.Password)) // In a real application, you should hash and salt passwords
        return null;

      return user;
    }

    private static bool CheckPassword(string password, string? passwordHash)
    {
      InsERT.Dodatki dodatki = new InsERT.Dodatki();
      var hash = dodatki.Szyfruj(password);
      if (!dodatki.SzyfrySaRowne(hash, passwordHash))
      {
        return false;
      }
      return true;
    }
  }
}
