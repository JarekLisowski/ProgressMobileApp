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
    public async Task<SaveDocumentResponse> PostInvoice(Domain.Api.Document document)
    {
      document.UserId = GetUserId();
      var result = await _navireoConnector.SaveDocument(document);
      return result;
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

    [HttpPost("pay")]
    public async Task<ApiResult> PostPayment(Payment payment)
    {
      var userId = GetUserId();
      if (userId != null)
      {
        try
        {
          var result = await _navireoConnector.AddPayment(payment, userId.Value);
          return result;
        }
        catch(Exception ex)
        {
          return new ApiResult
          {
            IsError = true,
            Message = ex.Message,
          };
        }
      }
      return new ApiResult
      {
        IsError = true,
        Message = "No user"
      };
    }

  }
}
