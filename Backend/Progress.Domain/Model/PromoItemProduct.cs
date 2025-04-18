namespace Progress.Domain.Model;

public class PromoItemProduct
{
    public int PromoItemId { get; set; }

    public string ProductCode { get; set; } = null!;

    public virtual PromoItem PromoItem { get; set; } = null!;
}
