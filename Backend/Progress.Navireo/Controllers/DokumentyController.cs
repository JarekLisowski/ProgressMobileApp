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

    public DokumentyController(DocumentManager documentManager, FinanceManager financeManager)
    {
      _documentManager = documentManager;
      _financeManager = financeManager;
    }

    [HttpPost("saveDocument")]
    public SaveDocumentResponse SaveDocument(Domain.Api.Document request)
    {
      try
      {
        var document = request.ToNavireoDocument();
        document.IsNew = true;
        var result = _documentManager.UpdateDocument(document);
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
        throw;
      }
    }

    [HttpPost("pay")]
    public ApiResult SettleDocument(PaymentRequest request)
    {
      if (request?.Payment == null)
        return new ApiResult
        {
          IsError = true,
          Message = "Brak danych"
        };

      try
      {
        var documentSettlement = request.Payment.ToNavireoDocumentSettlement();
        _financeManager.SettleDocument(request.OperatorId, documentSettlement);
        return new ApiResult
        {
          Message = "Zapłacono"
        };
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return new ApiResult
        {
          IsError = true,
          Message = ex.Message
        };
      }
    }
  }
}
