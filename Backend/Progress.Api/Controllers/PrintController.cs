using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Progress.BusinessLogic;
using Progress.Domain.Api.Response;

namespace Progress.Api.Controllers
{
  [Route("api/print")]
  [ApiController]
  public class PrintController : ApiControllerBase
  {
    IPrintService _printService;

    public PrintController(IMapper autoMapper,
      IServiceProvider serviceProvider,
      IPrintService printService
      )
      : base(serviceProvider)
    {
      _printService = printService;
    }

    [HttpGet("getPrintout/{requestId}")]
    public PrintDocumentResponse GetPrintout(string requestId)
    {
      var printout = _printService.GetPrintout(requestId);
      return new PrintDocumentResponse
      {
        Info = printout.DocNumber,
        Lines = printout.Data.Split("\r\n")
      };
    }

    [HttpPost("request/invoice/{dokId}")]
    public PrintRequestResponse RequestInvoicePrint(int dokId)
    {
      try
      {
        var guid = _printService.GenerateInvoicePrintout(dokId);
        return new PrintRequestResponse
        {
          Data = guid.ToString()
        };
      }
      catch (Exception ex)
      {
        return new PrintRequestResponse
        {
          IsError = true,
          Message = ex.Message,
        };
      }
    }

    [HttpPost("test-request/invoice/{dokId}")]
    public PrintDocumentResponse TestRequestInvoicePrint(int dokId)
    {
      try
      {
        var guid = _printService.GenerateInvoicePrintout(dokId);
        var printout = _printService.GetPrintout(guid.ToString());
        return new PrintDocumentResponse
        {
          Info = printout.DocNumber,
          Lines = printout.Data.Split("\r\n")
        };
      }
      catch (Exception ex)
      {
        return new PrintDocumentResponse
        {
          Info = ex.Message,
          Lines = []
        };
      }
    }

    [HttpPost("request/cashReceipt/{nzId}")]
    public PrintRequestResponse RequestCashReceipt(int nzId)
    {
      try
      {
        var guid = _printService.GenerateCashReceiptPrintout(nzId);
        return new PrintRequestResponse
        {
          Data = guid.ToString()
        };
      }
      catch (Exception ex)
      {
        return new PrintRequestResponse
        {
          IsError = true,
          Message = ex.Message,
        };
      }
    }

    [HttpPost("test-request/cashReceipt/{nzId}")]
    public PrintDocumentResponse TestRequestCashReceiptPrint(int nzId)
    {
      try
      {
        var guid = _printService.GenerateCashReceiptPrintout(nzId);
        var printout = _printService.GetPrintout(guid.ToString());
        return new PrintDocumentResponse
        {
          Info = printout.DocNumber,
          Lines = printout.Data.Split("\r\n")
        };
      }
      catch (Exception ex)
      {
        return new PrintDocumentResponse
        {
          Info = ex.Message,
          Lines = []
        };
      }
    }
  }
}
