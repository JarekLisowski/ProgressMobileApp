namespace Progress.Navireo.Navireo
{
  public class NavireoService : BackgroundService
  {
    NavireoApplication _navireoApplication;
    ILogger<NavireoService> _logger;
    public NavireoService(IServiceProvider serviceProvider, ILogger<NavireoService> logger)
    {
      _logger = logger;
      _navireoApplication = serviceProvider.GetRequiredService<NavireoApplication>();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
      return Task.Run(Main);
    }

    private void Main()
    {
      try
      {
        var navireo = _navireoApplication.GetNavireo();
        var ok = navireo.Wersja;
        var op = navireo.OperatorId;
        var magId = navireo.MagazynId;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Nie można uruchomić Navireo");
      }
    }
  }
}
