using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Progress.BusinessLogic;
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
    DocumentManager _documentManager;

    public DokumentyController(IMapper autoMapper,
      IServiceProvider serviceProvider,
      NavireoConnector navireoConnector,
      DocumentRepository documentRepository,
      DocumentManager documentManager)
      : base(serviceProvider)
    {
      _mapper = autoMapper;
      _navireoConnector = navireoConnector;
      _documentRepository = documentRepository;
      _documentManager = documentManager;
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

    [HttpGet("document/{id}")]
    public DocumentResponse GetInvoice(int id)
    {
      var data = _documentRepository.GetDocument(id);
      return new DocumentResponse()
      {
        Data = [_mapper.Map<Document>(data)]
      };
    }

    [HttpPost("send")]
    public async Task<SaveDocumentResponse> PostInvoice(Domain.Api.Document document)
    {
      try
      {
        document.UserId = GetUserId();
        var result = await _navireoConnector.SaveDocument(document);
        return result;
      } catch(Exception ex)
      {
        return new SaveDocumentResponse
        {
          IsError = true,
          Message = ex.Message
        };
      }
    }

    [HttpGet("orders/{customerId}")]
    public DocumentResponse GetOrders(int? customerId, string? dateFrom = null, string? dateTo = null, int pageSize = 100, int page = 1)
    {
      var data = _documentRepository.GetDocuments(16, customerId);
      return new DocumentResponse()
      {
        Data = _mapper.Map<Document[]>(data)
      };
    }

    [HttpGet("internal-orders")]
    public DocumentResponse GetInternalOrders(string? dateFrom = null, string? dateTo = null, int pageSize = 100, int page = 1)
    {
      var userId = GetUserId();
      if (userId != null)
      {
        var data = _documentManager.GetInternalOrders(userId.Value);
        return new DocumentResponse()
        {
          Data = _mapper.Map<Document[]>(data)
        };
      }
      return new DocumentResponse();
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
