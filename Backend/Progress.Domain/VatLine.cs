namespace Progress.Domain
{
  public class VatLine
  {
    public string RateName { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public decimal Tax { get; set; }
    public decimal Net { get; set; }
    public decimal Gross { get; set; }
  }
}
