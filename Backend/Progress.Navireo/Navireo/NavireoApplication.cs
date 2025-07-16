using Progress.Navireo.Helpers;
using System.Runtime.InteropServices;

namespace Progress.Navireo.Navireo
{
  /// <summary>
  /// Navireo Application
  /// </summary>
  public class NavireoApplication
  {
    IServiceProvider _serviceProvider;
    IConfiguration _configurationProvider;
    Helpers.Logger Logger;
    InsERT.Navireo? Navireo;

    public NavireoApplication(IServiceProvider serviceProvider, IConfiguration configurationProvider)
    {
      _configurationProvider = configurationProvider;
      _serviceProvider = serviceProvider;
    }

    object getNavireoLock = new object();

    /// <summary>
    /// Zwraca instancję Navireo 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public InsERT.Navireo? GetNavireo()
    {
      lock (getNavireoLock)
      {
        if (Navireo == null)
        {
          //  var dbServer = _configurationProvider.GetValue<string>("Navireo:dbServer");
          //  var dbName = _configurationProvider.GetValue<string>("Navireo:dbName");
          //  var dbUser = _configurationProvider.GetValue<string>("Navireo:dbUser");
          //  var dbPass = _configurationProvider.GetValue<string>("Navireo:dbPass");
          //  var navUserId = _configurationProvider.GetValue<int>("Navireo:navUserId");
          //  var navPass = _configurationProvider.GetValue<string>("Navireo:navPass");
          //  RunNavireo(dbServer, dbName, dbUser, dbPass, navUserId, navPass);
          var startFile = _configurationProvider.GetValue<string>("Navireo:startFile");
          if (startFile != null)
          {
            RunNavireo(startFile);
          }
        }
        return Navireo;
      }
    }

    /// <summary>
    /// Zamyka instancję navireo
    /// </summary>
    /// <param name="oNavireo"></param>
    /// <returns></returns>
    public Task ZamknijNavireo(InsERT.Navireo oNavireo)
    {
      //Logger.Log(LogType.Message, $"Zamykam Navireo.", 0);
      return Task.Run(() =>
          {
            try
            {
              int counter = 0;
              while (oNavireo.Zajeta && counter < 10)
              {
                counter++;
                Thread.Sleep(3000);
        //        Logger.Log(LogType.Message, $"Zamykam Navireo (próba {counter})", 0);
              }
              if (oNavireo.Zajeta)
          //      Logger.Log(LogType.Warning, "Zamykam Navireo mimo że jest zajęty.", 0);
              oNavireo.Zakoncz();
            }
            catch (Exception e)
            {
            //  Logger.Log(LogType.Exception, "Błąd podczas zamykania Navireo: " + e.Message, 0);
            }
          });
    }

    /// <summary>
    /// Uruchamia instancję Navireo
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    private void RunNavireo(string server, string databaseName, string dbUser, string dbPassword, int navUserId, string navPassword)
    {
      Navireo = new InsERT.Navireo();
      try
      {
        if (string.IsNullOrEmpty(dbUser))
          Navireo.Zaloguj(server, InsERT.AutentykacjaEnum.gtaAutentykacjaWindows, "", "", databaseName, null, null, InsERT.AutentykacjaEnum.gtaAutentykacjaMieszana, navUserId, navPassword);
        else
          Navireo.Zaloguj(server, InsERT.AutentykacjaEnum.gtaAutentykacjaMieszana, dbUser, dbPassword, databaseName, null, null, InsERT.AutentykacjaEnum.gtaAutentykacjaMieszana, navUserId, navPassword);
      }
      catch (Exception e)
      {
        try
        {
          Navireo.Zakoncz();
        }
        catch (Exception ex)
        {
        //  Logger.Log(LogType.Exception, e);
        }
        if (e.HResult == -2147352567 || e.Message.Contains("przekroczony limit wykupionych licencji"))
        {
          Logger.Log(LogType.Exception, "Przekroczony limit licencji", navUserId);
          //ClearTable_log_OdrzLicencje();
          //throw;
        }
        Navireo = null;
      }
    }

    private static Dictionary<string, InsERT.Navireo> navireoApp;

    static public string SkrotNavireo { get; set; }

    private void RunNavireo(string skrot)
    {
      Navireo = null;
      if (!skrot.StartsWith("\\") || skrot[1] != ':')
      {
        skrot = Directory.GetCurrentDirectory() + "\\" + skrot;
      }
      var oNavireo = Marshal.BindToMoniker(skrot);
      Navireo = oNavireo as InsERT.Navireo;
    }

    public void ZamknijNavireo()
    {
      if (Navireo != null)
      {
        Navireo.Zakoncz();
        Navireo = null;
      }
    }

  }
}