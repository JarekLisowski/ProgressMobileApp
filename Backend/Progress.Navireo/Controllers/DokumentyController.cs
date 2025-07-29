using Microsoft.AspNetCore.Mvc;
using Progress.Domain.Api;
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

    [HttpPost("updateDocument")]
    public object UpdateDocument(Domain.Api.Document request)
    {
      try
      {
        var document = request.ToNavireoDocument();
        document.IsNew = true;
        _documentManager.UpdateDocument(document);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        throw;
      }
      return "OK";
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
