namespace Progress.Navireo.Navireo
{
  public class NavireoService : BackgroundService
  {
    NavireoApplication _navireoApplication;
    public NavireoService(IServiceProvider serviceProvider)
    {
      _navireoApplication = serviceProvider.GetRequiredService<NavireoApplication>();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
      return Task.Run(Main);
    }

    private void Main()
    {
      var navireo = _navireoApplication.GetNavireo();
      var ok = navireo.Wersja;
      var op = navireo.OperatorId;
      var magId = navireo.MagazynId;
    }
  }
}
