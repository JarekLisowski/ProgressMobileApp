namespace Progress.Database.Custom
{
  public class TwSearchResult
  {
    public double Rank { get; set; } // Assuming Rank is a double or float
    public int TwId { get; set; }
    public string TwSymbol { get; set; } = string.Empty;
    public string TwNazwa { get; set; } = string.Empty;
  }
}
