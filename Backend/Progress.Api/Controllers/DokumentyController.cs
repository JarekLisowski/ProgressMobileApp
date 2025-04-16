using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Progress.Api.Controllers
{
	[Route("api/document")]
	[ApiController]
	public class DokumentyController : ControllerBase
	{
		[HttpGet("invoice/{id}")]
		public string GetInvoice(long id)
		{
			return "OK";
		}

		[HttpGet("invoices")]
		public string GetInvoices(long? customerId, string? dateFrom, string dateTo, int pageSize = 100, int page = 1)
		{
			return "OK";
		}

		[HttpPost("invoice")]
		public string PostInvoice(long id) 
		{
			return "OK";
		}

		[HttpGet("order/{id}")]
		public string GetOrder(long id)
		{
			return "OK";
		}

		[HttpGet("orders")]
		public string GetOrders(long? customerId, string? dateFrom, string dateTo, int pageSize = 100, int page = 1)
		{
			return "OK";
		}

		[HttpPost("order")]
		public string PostOrder(long id)
		{
			return "OK";
		}

		[HttpPost("payment")]
		public string PostPayment(long id)
		{
			return "OK";
		}

	}
}
