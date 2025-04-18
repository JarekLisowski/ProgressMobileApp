namespace Progress.Domain.Model;

public class PromoSet
{
  public int Id { get; set; }

  public bool IsDeleted { get; set; }

  public DateTime? ValidFrom { get; set; }

  public DateTime? ValidUntil { get; set; }

  public string Name { get; set; } = null!;

  public DateTime DataChange { get; set; }

  public DateTime? GroupChangeDate { get; set; }

  public string? Image { get; set; }

  public byte[]? Img { get; set; }

  public byte Typ { get; set; }

  public virtual ICollection<PromoItem> Items { get; set; } = new List<PromoItem>();
}
