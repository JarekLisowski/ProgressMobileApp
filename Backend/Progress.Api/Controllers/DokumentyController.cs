using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Progress.BusinessLogic;
using Progress.Domain.Api;
using Progress.Domain.Api.Response;
using Progress.Infrastructure.Database.Repository;
using System.Globalization;

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

    [HttpGet("my-invoices/{customerId}")]
    public DocumentResponse GetMyInvoices(int? customerId)
    {
      var user = GetUser();
      if (user != null && user.CechaId != null)
      {
        var fromDate = DateTime.Today.AddYears(-1);
        var data = _documentRepository.GetDocumentsOwnCustomers(2, user.CechaId.Value, fromDate);
        return new DocumentResponse()
        {
          Data = _mapper.Map<Document[]>(data)
        };
      }
      return new DocumentResponse
      {
        IsError = true,
        Message = "Not logged in"
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

    [HttpGet("my-orders/{customerId}")]
    public DocumentResponse GetMyOrders(int? customerId, string? dateFrom = null, string? dateTo = null, int pageSize = 100, int page = 1)
    {
      var user = GetUser();
      if (user != null && user.CechaId != null)
      {
        var fromDate = DateTime.Today.AddYears(-1);
        var data = _documentRepository.GetDocumentsOwnCustomers(16, user.CechaId.Value, fromDate);
        return new DocumentResponse()
        {
          Data = _mapper.Map<Document[]>(data)
        };
      }
      return new DocumentResponse
      {
        IsError = true,
        Message = "Not logged in"
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

    [HttpGet("sale-summary")]
    public async Task<SaleSummaryResponse> GetSaleSummary(string from, string to)
    {
      try
      {
        var userId = GetUserId();
        if (userId != null)
        {
          var dfp = CultureInfo.InvariantCulture.DateTimeFormat;
          if (DateTime.TryParse(from, dfp, out var dateFrom)
            && DateTime.TryParse(to, dfp, out var dateTo))
          {
            var result = await _navireoConnector.GetSaleSummary(userId.Value, dateFrom, dateTo, "PLN");
            return result;
          }
          return new SaleSummaryResponse
          {
            IsError = true,
            Message = "Incorrect Date format"
          };
        }
        return new SaleSummaryResponse
        {
          IsError = true,
          Message = "Incorrect user"
        };
      }
      catch (Exception ex)
      {
        return new SaleSummaryResponse
        {
          IsError = true,
          Message = ex.Message
        };
      }
    }

  }
}
