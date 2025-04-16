namespace Progress.Domain.Api.Request
{
    public class ApiRequest
    {
    }

    public class ApiListRequest
    {
        public int? PageNo { get; set; }
        public int? PageSize { get; set; }
    }
}
