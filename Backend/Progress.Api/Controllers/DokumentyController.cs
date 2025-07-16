using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Progress.Domain.Api.Response;

namespace Progress.Api.Controllers
{
	[Authorize]
	[Route("api/document")]
	[ApiController]
	public class DokumentyController : ApiControllerBase
  {
    IMapper _mapper;
		NavireoConnector _navireoConnector;

    public DokumentyController(IMapper autoMapper, IServiceProvider serviceProvider, NavireoConnector navireoConnector)
  : base(serviceProvider)
    {
      _mapper = autoMapper;
			_navireoConnector = navireoConnector;
    }

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
		public async Task<ApiResult<string>> PostInvoice(Domain.Api.Document document) 
		{
			document.UserId = GetUserId();
			var result = await _navireoConnector.UpdateDocument(document);
			return new ApiResult<string>(result);
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
