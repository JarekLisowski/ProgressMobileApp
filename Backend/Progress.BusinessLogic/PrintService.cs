using Fluid;
using Fluid.Values;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;
using System.Globalization;

namespace Progress.BusinessLogic
{
  public class PrintService : BackgroundService, IPrintService
  {
    CancellationToken _stoppingToken;
    ConcurrentDictionary<Guid, Printout> printOuts = new ConcurrentDictionary<Guid, Printout>();
    ConcurrentDictionary<string, IFluidTemplate> templates = new ConcurrentDictionary<string, IFluidTemplate>();
    IServiceProvider _serviceProvider;

    public PrintService(IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
      _stoppingToken = stoppingToken;
      return Task.Run(MainTask);
    }

    private void MainTask()
    {
      do
      {
        _stoppingToken.WaitHandle.WaitOne(TimeSpan.FromSeconds(10));
      } while (!_stoppingToken.IsCancellationRequested);
    }

    public Printout? GetPrintout(string guid)
    {
      var g = new Guid(guid);
      if (printOuts.TryGetValue(g, out var printout))
        return printout;
      return null;
    }

    public Guid GenerateInvoicePrintout(int dokId)
    {
      var documentManager = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<DocumentManager>();
      var doc = documentManager.GetDocument(dokId);
      if (doc != null)
      {
        return GeneratePrintout("invoice", doc, doc.Number);
      }
      throw new Exception($"Brak faktury {dokId}");
    }

    public Guid GenerateCashReceiptPrintout(int nzId)
    {
      var financeManager = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<FinanceManager>();
      var doc = financeManager.GetCashReceipt(nzId);
      if (doc != null)
      {
        return GeneratePrintout("cashReceipt", doc, doc.Number);
      }
      throw new Exception($"Brak dokumentu KP. {nzId}");
    }

    private Guid GeneratePrintout(string templateName, object data, string docNumber)
    {
      var template = GetFuidTemplate(templateName);
      if (template != null)
      {
        var options = new TemplateOptions();
        var context = new TemplateContext(data);
        var output = template.Render(context);
        var guid = Guid.NewGuid();
        printOuts[guid] = new Printout { Data = output, DocNumber = docNumber };
        return guid;
      }
      throw new Exception("Brak wzorca wydruku");
    }



    private IFluidTemplate? GetFuidTemplate(string templateName)
    {
      if (!templates.TryGetValue(templateName, out var template))
      {
        var templateFile = File.ReadAllText($"Templates\\{templateName}.txt");
        var parser = new FluidParser();
        TemplateOptions.Default.CultureInfo = new CultureInfo("pl-PL");
        TemplateOptions.Default.MemberAccessStrategy = new UnsafeMemberAccessStrategy();
        TemplateOptions.Default.Filters.AddFilter("alignRight", AlighRight);
        TemplateOptions.Default.Filters.AddFilter("alignLeft", AlighLeft);
        template = parser.Parse(templateFile);
      }
      ;
      return template;
    }

    ValueTask<FluidValue> AlighRight(FluidValue input, FilterArguments arguments, TemplateContext context)
    {
      var text = input.ToStringValue();
      var fWidth = arguments.At(0);
      var width = (int)fWidth.ToNumberValue();
      var result = text.PadLeft(width);
      if (result.Length > width)
        result = result.Substring(0, width);
      return new StringValue(result);
    }

    ValueTask<FluidValue> AlighLeft(FluidValue input, FilterArguments arguments, TemplateContext context)
    {
      var text = input.ToStringValue();
      var fWidth = arguments.At(0);
      var width = (int)fWidth.ToNumberValue();
      var result = text.PadRight(width);
      if (result.Length > width)
        result = result.Substring(0, width);
      return new StringValue(result);
    }




  }
}
