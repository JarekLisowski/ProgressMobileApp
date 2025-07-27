using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Progress.Domain.Api;
using Progress.Domain.Api.Response;
using Progress.Infrastructure.Database.Repository;

namespace Progress.Api.Controllers
{
  [Authorize]
  [Route("api/document")]
  [ApiController]
  public class DokumentyController : ApiControllerBase
  {
    IMapper _mapper;
    NavireoConnector _navireoConnector;
    DocumentRepository _documentRepository;

    public DokumentyController(IMapper autoMapper,
      IServiceProvider serviceProvider,
      NavireoConnector navireoConnector,
      DocumentRepository documentRepository)
      : base(serviceProvider)
    {
      _mapper = autoMapper;
      _navireoConnector = navireoConnector;
      _documentRepository = documentRepository;
    }

    [HttpGet("invoices/{customerId}")]
    public DocumentResponse GetInvoices(int? customerId)
    {
      var data = _documentRepository.GetDocuments(2, customerId);
      return new DocumentResponse()
      {
        Data = _mapper.Map<Document[]>(data)
      };
    }

    [HttpGet("invoice/{id}")]
    public DocumentResponse GetInvoice(int id)
    {
      var data = _documentRepository.GetDocument(id);
      return new DocumentResponse()
      {
        Data = [_mapper.Map<Document>(data)]
      };
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
