using Fluid;
using Fluid.Values;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Progress.Domain.Extensions;
using Progress.Infrastructure.Database.Repository;
using System.Collections.Concurrent;

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
      var template = GetFuidTemplate("invoice");
      if (template != null)
      {
        var doc = documentManager.GetDocument(dokId);
        if (doc != null)
        {
          var context = new TemplateContext(doc);
          var output = template.Render(context);
          var guid = Guid.NewGuid();
          printOuts[guid] = new Printout { Data = output, DocNumber = doc.Number };
          return guid;
        }
        throw new Exception("Brak dokumentu");
      }
      throw new Exception("Brak wzorca wydruku");
    }

    private IFluidTemplate? GetFuidTemplate(string templateName)
    {
      if (!templates.TryGetValue(templateName, out var template))
      {
        var templateFile = File.ReadAllText($"Templates\\{templateName}.txt");
        var parser = new FluidParser();
        var options = new TemplateOptions();
        TemplateOptions.Default.MemberAccessStrategy = new UnsafeMemberAccessStrategy();
        TemplateOptions.Default.Filters.AddFilter("alignRight", AlighRight);
        TemplateOptions.Default.Filters.AddFilter("alignLeft", AlighLeft);
        //options.MemberAccessStrategy.Register<Document>();
        //options.MemberAccessStrategy.Register<Customer>();
        //options.MemberAccessStrategy.Register<DocumentItem>();
        //options.MemberAccessStrategy.Register<VatLine>();
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

    private void MainTask()
    {
      do
      {

        _stoppingToken.WaitHandle.WaitOne(TimeSpan.FromSeconds(10));
      } while (!_stoppingToken.IsCancellationRequested);
    }

 
  }
}
