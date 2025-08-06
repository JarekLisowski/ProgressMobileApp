namespace Progress.Navireo.Domain
{
  public class InfoResponse
  {
    public string Status => "OK";
    public string Navireo {  get; set; }
    public DateTime DateTime { get; set; } = DateTime.Now;
  }
}
