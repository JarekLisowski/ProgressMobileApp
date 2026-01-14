using Microsoft.AspNetCore.Mvc;
using Progress.Domain.Api.Request;
using Progress.Domain.Api.Response;
using Progress.Domain.Extensions;
using Progress.Navireo.Managers;

namespace Progress.Navireo.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DokumentyController : ControllerBase
  {
    DocumentManager _documentManager;
    FinanceManager _financeManager;
    ILogger<DokumentyController> _logger;

    public DokumentyController(DocumentManager documentManager, FinanceManager financeManager, ILogger<DokumentyController> logger)
    {
      _documentManager = documentManager;
      _financeManager = financeManager;
      _logger = logger;
    }

    [HttpPost("saveDocument")]
    public SaveDocumentResponse SaveDocument(Progress.Domain.Api.Document request)
    {
      try
      {
        var document = request.ToNavireoDocument();
        document.IsNew = true;
        var result = _documentManager.AddOrUpdateDocument(document);
        return new SaveDocumentResponse
        {
          DocumentId = result.DocumentId,
          PayDocumentId = result.PayDocumentId,
          DocumentNumber = result.DocumentNumber,
          DocumentType = result.DocumentType
        };
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        _logger.LogError(ex, "SaveDocument Error");
        return new SaveDocumentResponse
        {
          IsError = true,
          Message = ex.Message,
        };
      }
    }

    [HttpPost("pay")]
    public SaveDocumentResponse SettleDocument(PaymentRequest request)
    {
      if (request?.Payment == null)
        return new SaveDocumentResponse
        {
          IsError = true,
          Message = "Brak danych"
        };

      try
      {
        var documentSettlement = request.Payment.ToNavireoDocumentSettlement();
        var wplataId = _financeManager.SettleDocument(request.OperatorId, documentSettlement);
        return new SaveDocumentResponse
        {
          Message = "Zapłacono",
          PayDocumentId = wplataId ?? 0
        };
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return new SaveDocumentResponse
        {
          IsError = true,
          Message = ex.Message
        };
      }
    }
  }
}
