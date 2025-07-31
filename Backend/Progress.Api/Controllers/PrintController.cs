using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Progress.BusinessLogic;
using Progress.Domain.Api.Response;
using Progress.Infrastructure.Database.Repository;
using System;

namespace Progress.Api.Controllers
{
  [Route("api/print")]
  [ApiController]
  public class PrintController : ApiControllerBase
  {
    IPrintService _printService;
    IMapper _mapper;
    NavireoConnector _navireoConnector;
    DocumentRepository _documentRepository;

    public PrintController(IMapper autoMapper,
      IServiceProvider serviceProvider,
      IPrintService printService,
      NavireoConnector navireoConnector,
      DocumentRepository documentRepository)
      : base(serviceProvider)
    {
      _mapper = autoMapper;
      _navireoConnector = navireoConnector;
      _documentRepository = documentRepository;
      _printService = printService;
    }

    [HttpGet("invoice/{requestId}")]
    public PrintDocumentResponse Invoice(string requestId)
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
  }
}
