using Microsoft.AspNetCore.Mvc;
using Progress.Domain.Api.Request;
using Progress.Domain.Api.Response;
using Progress.Navireo.Managers;
using Progress.Domain.Extensions;

namespace Progress.Navireo.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class FinanceController : ControllerBase
  {
    ILogger<FinanceController> _logger;
    FinanceManager _financeManager;

    public FinanceController(FinanceManager financeManager, ILogger<FinanceController> logger)
    {
      _financeManager = financeManager;
      _logger = logger;
    }

    [HttpPost("getSaleSummary")]
    public SaleSummaryResponse GetSaleSummary(SaleSummaryRequest saleSummaryRequest)
    {
      try
      {
        var result = _financeManager.GetSaleSummary(saleSummaryRequest.OperatorId, saleSummaryRequest.DateFrom, saleSummaryRequest.DateTo, saleSummaryRequest.CurrencyCode);
        var summary = new Progress.Domain.Api.SaleSummary
        {
          CurrentCash = result.CurrentCash,
          ReceivedCash = result.ReceivedCash,
          SaleCashGross = result.SaleCashGross,
          SaleGross = result.SaleGross,
          SaleNet = result.SaleNet,
          DocumentSummary = result.DocumentSummary.Select(it => new Progress.Domain.Api.DocumentSummary
          {
            Count = it.Count,
            TotalCashGross = it.TotalCashGross,
            TotalGross = it.TotalGross,
            TotalNet = it.TotalNet,
            Type = it.Type,
            TypeName = it.Type.GetName()
          }).ToList()
        };
        return new SaleSummaryResponse
        {
          Data = summary
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
