namespace Progress.Domain.Api;

public partial class ProductImage
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public byte[]? Image { get; set; }

    public bool Primary { get; set; }


}
