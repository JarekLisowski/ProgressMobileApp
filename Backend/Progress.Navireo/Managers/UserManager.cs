namespace Progress.Navireo.Managers
{
  public class UserManager
  {
    public static bool CheckPassword(string password, string? passwordHash)
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
