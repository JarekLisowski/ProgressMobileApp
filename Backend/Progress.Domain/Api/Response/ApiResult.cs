namespace Progress.Domain.Api.Response
{
	public class ApiResult
	{
		public bool IsError { get; set; }
		public string Message { get; set; } = string.Empty;
		public bool MorePages { get; set; }
		public int? TotalPages { get; set; }
		public int? ItemsPerPage { get; set; }
	}

	public class ApiResult<T_PAYLOAD> : ApiResult
			where T_PAYLOAD : class
	{
		public T_PAYLOAD? Data { get; set; }
	}
}
