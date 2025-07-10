namespace Progress.Domain.Api
{
  public class Login
  {
    public string Token { get; set; } = string.Empty;
    public DateTime ExpirationDate { get; set; }
    public User? User { get; set; }
  }
}
