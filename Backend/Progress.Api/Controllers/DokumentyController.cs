using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Progress.BusinessLogic;
using Progress.Domain.Api;
using Progress.Domain.Api.Response;
using Progress.Infrastructure.Database.Repository;
using System.Globalization;
using System.Runtime.Intrinsics.Arm;

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
    public DocumentResponse GetInvoices(int? customerId, string? from = null, string? to = null)
    {
      var dfp = CultureInfo.InvariantCulture.DateTimeFormat;
      if (from == null || !DateTime.TryParse(from, dfp, out var dateFrom))
        dateFrom = DateTime.Today.Subtract(TimeSpan.FromDays(365));
      if (to == null || !DateTime.TryParse(to, dfp, out var dateTo))
        dateTo = DateTime.Today;
      var data = _documentRepository.GetDocuments(2, customerId, dateFrom, dateTo);
      return new DocumentResponse()
      {
        Data = _mapper.Map<Document[]>(data)
      };
    }

    [HttpGet("my-invoices/{customerId}")]
    public DocumentResponse GetMyInvoices(int? customerId, string from, string to)
    {
      var user = GetUser();
      if (user != null && user.CechaId != null)
      {
        var dfp = CultureInfo.InvariantCulture.DateTimeFormat;
        if (from == null || !DateTime.TryParse(from, dfp, out var dateFrom))
          dateFrom = DateTime.Today.Subtract(TimeSpan.FromDays(365));
        if (to == null || !DateTime.TryParse(to, dfp, out var dateTo))
          dateTo = DateTime.Today;
        var data = _documentRepository.GetDocumentsOwnCustomers(2, user.CechaId.Value, dateFrom, dateTo);
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
    public DocumentResponse GetOrders(int? customerId, string? from = null, string? to = null, int pageSize = 100, int page = 1)
    {
      var dfp = CultureInfo.InvariantCulture.DateTimeFormat;
      if (from == null || !DateTime.TryParse(from, dfp, out var dateFrom))
        dateFrom = DateTime.Today.Subtract(TimeSpan.FromDays(365));
      if (to == null || !DateTime.TryParse(to, dfp, out var dateTo))
        dateTo = DateTime.Today;
      var data = _documentRepository.GetDocuments(16, customerId, dateFrom, dateTo);
      return new DocumentResponse()
      {
        Data = _mapper.Map<Document[]>(data)
      };
    }

    [HttpGet("my-orders/{customerId}")]
    public DocumentResponse GetMyOrders(int? customerId, string? from = null, string? to = null, int pageSize = 100, int page = 1)
    {
      var user = GetUser();
      if (user != null && user.CechaId != null)
      {
        var dfp = CultureInfo.InvariantCulture.DateTimeFormat;
        if (from == null || !DateTime.TryParse(from, dfp, out var dateFrom))
          dateFrom = DateTime.Today.Subtract(TimeSpan.FromDays(365));
        if (to == null || !DateTime.TryParse(to, dfp, out var dateTo))
          dateTo = DateTime.Today;
        var data = _documentRepository.GetDocumentsOwnCustomers(16, user.CechaId.Value, dateFrom, dateTo);
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
    public async Task<SaveDocumentResponse> PostPayment(Payment payment)
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
          return new SaveDocumentResponse
          {
            IsError = true,
            Message = ex.Message,
          };
        }
      }
      return new SaveDocumentResponse
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
