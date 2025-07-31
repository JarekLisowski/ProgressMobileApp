namespace Progress.BusinessLogic
{
  public class Printout
  {
    public DateTime DateTime { get; set; } = DateTime.Now;
    public string Data {  get; set; } = string.Empty;
    public string DocNumber { get; set; } = string.Empty;
  }
}
