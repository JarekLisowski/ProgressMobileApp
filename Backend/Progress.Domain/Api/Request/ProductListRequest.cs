namespace Progress.Domain.Api.Request
{
    public class ProductListRequest : ApiListRequest
    {
        public int? CategoryId { get; set; }
    }
}
