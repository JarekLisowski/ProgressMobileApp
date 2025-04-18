namespace Progress.Domain.Api;

public class PromoItemProduct
{
    public int PromoItemId { get; set; }

    public string ProductCode { get; set; } = null!;

    public PromoItem PromoItem { get; set; } = null!;
}
