using System.Xml.Serialization;
using System.Diagnostics;

namespace Progress.Navireo.Helpers
{
  /// <summary>
  /// Logger
  /// </summary>
  public class Logger
  {
    IServiceProvider _serviceProvider;
    public Logger(IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider;
    }

    private Database.NavireoDbContext CreateDbContext()
      => _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<Database.NavireoDbContext>();

    public string LogTypNazwa(LogType typ)
    {
      switch (typ)
      {
        case LogType.Warning:
          {
            return "Ostrzeżenie";

          }
        case LogType.Message:
          {
            return "Informacja";

          }
        case LogType.Exception:
          {
            return "Błąd";

          }
        case LogType.BusinessUpdate:
          {
            return "Kontrahent - zapis";

          }
        case LogType.DocumentUpdate:
          {
            return "Dokument - zapis";

          }
        case LogType.SqliteEvent:
          {
            return "Paczka Sqlite";

          }
        case LogType.TokenException:
          {
            return "Token";

          }
        case LogType.BusinessRead:
          {
            return "Kontrahenci - odczyt";

          }
        case LogType.DocumentRead:
          {
            return "Dokument - odczyt";

          }
        case LogType.ProductRead:
          {
            return "Towary - odczyt";

          }
        case LogType.ConfigurationRead:
          {
            return "Ustawienia";

          }
        case LogType.FinanceUpdate:
          {
            return "Finanse - zapis";

          }
        case LogType.FinanceRead:
          {
            return "Finanse - odczyt";
          }
        case LogType.DiscountSet:
          {
            return "Promocje - odczyt";
          }
        default:
          {
            return typ.ToString();
          }
      }
    }

    /// <summary>
    /// Dodaje wpis do logu
    /// </summary>
    /// <param name="type">Typ</param>
    /// <param name="e">Exception</param>
    /// <param name="requestType">Request type</param>
    /// <param name="request">Request</param>
    /// <param name="token">User token</param>
    public Task Log(LogType type, Exception e, string token = null, object request = null, object response = null, Stopwatch stopwatch = null, DateTime? time = null)
    {
      return Task.Run(() =>
      {
        using(var dbContext = CreateDbContext())
        {
          try
          {
            int operatorId = 0;
            var toAdd = new Database.IfxApiLog
            {
              Type = (int)type,
              Message = e.Message != null ? e.Message : null,
              InnerException = e != null ? GetInnerException(e.InnerException) : null,
              StackTrace = e.StackTrace != null ? e.StackTrace : null,
              Date = time != null ? (DateTime)time : DateTime.Now,
              UzId = operatorId
            };
            if (request != null)
            {
              toAdd.XmlRequest = Serialize(request.GetType(), request);
            }
            if (response != null)
            {
              toAdd.XmlResponse = Serialize(response.GetType(), response);
            }
            if (stopwatch != null)
            {
              toAdd.ConsumedTime = stopwatch.Elapsed.Ticks;
            }
            dbContext.IfxApiLogs.Add(toAdd);
            dbContext.SaveChanges();
          }
          catch (Exception ex)
          {
            var toAdd = new Database.IfxApiLog
            {
              Type = (int)type,
              Message = "Logger exception" + ex.Message,
              Date = DateTime.Now,
            };
            dbContext.IfxApiLogs.Add(toAdd);
            dbContext.SaveChanges();
          }
        }
      });

    }

    /// <summary>
    /// Pobiera rekurencyjne inner exception
    /// </summary>
    /// <param name="e">Exception</param>
    /// <returns>Inner Exceptions</returns>
    private static string GetInnerException(Exception e)
    {
      string result = "";
      if (e != null)
      {
        result = e.Message + " ";

        if (e.InnerException != null)
          return result + " " + GetInnerException(e.InnerException);
      }
      return result;
    }

    /// <summary>
    /// Dodaje wpis w logu
    /// </summary>
    /// <param name="type">Typ zdarzenia</param>
    /// <param name="message">Informacja</param>
    /// <param name="token">Token użytkownika</param>
    /// <param name="request">Request</param>
    /// <param name="response">Response</param>
    /// <param name="stopwatch">Czas wykonywania operacji</param>
    /// <param name="dateTime">Czas wykonania operacji</param>
    /// <returns></returns>
    public Task Log(LogType type, string message, string token = null, object request = null, object response = null, Stopwatch stopwatch = null, DateTime? dateTime = null)
    {
      return Task.Run(() =>
      {
        int operatorId = 0;
        Log(type, message, operatorId, request, response, stopwatch, dateTime);
      });
    }


    /// <summary>
    /// Dodaje wspis do logu
    /// </summary>
    /// <param name="type">Typ</param>
    /// <param name="message">Wiadomość do zapisu</param>
    /// <param name="operatorId">Id użytkownika</param>
    /// <param name="request">Request</param>
    /// <param name="response">Response</param>
    /// <param name="stopwatch">Stopwatch</param>
    /// <param name="dateTime">Czas</param>
    /// <returns>Task</returns>
    public Task Log(LogType type, string message, int operatorId = 0, object request = null, object response = null, Stopwatch stopwatch = null, DateTime? dateTime = null)
    {
      return Task.Run(() =>
      {
        using (var dbContext = CreateDbContext())
        {
          try
          {
            var toAdd = new Database.IfxApiLog
            {
              Type = (int)type,
              Message = message,
              Date = dateTime != null ? (DateTime)dateTime : DateTime.Now,
              UzId = operatorId
            };
            if (request != null)
            {
              toAdd.XmlRequest = Serialize(request.GetType(), request);
            }
            if (response != null)
            {
              toAdd.XmlResponse = Serialize(response.GetType(), response);
            }
            if (stopwatch != null && stopwatch.Elapsed != null)
            {
              toAdd.ConsumedTime = stopwatch.Elapsed.Ticks;
            }
            dbContext.IfxApiLogs.Add(toAdd);
            dbContext.SaveChanges();
          }
          catch (Exception ex)
          {
            var toAdd = new Database.IfxApiLog
            {
              Type = (int)type,
              Message = "Logger exception" + ex.Message,
              Date = DateTime.Now,
            };
            dbContext.IfxApiLogs.Add(toAdd);
            dbContext.SaveChanges();
          }
        }
      });
    }

    /// <summary>
    /// Zwraca listę wpisów w logu
    /// </summary>
    /// <param name="id">Jeżeli wypełnione to zwraca konkretny wpis</param>
    /// <returns>Lista wpisów w logu</returns>
    public LogItem GetLogItem(int id)
    {
      using (var dbContext = CreateDbContext())
      {
        var users = dbContext.PdUzytkowniks.ToList();

        var item = dbContext.IfxApiLogs.FirstOrDefault(x => x.Id == id);

        return new LogItem
        {
          Id = item.Id,
          LogType = (LogType)item.Type,
          Date = item.Date != null ? item.Date : new DateTime(),
          Message = item.Message,
          innerException = item.InnerException,
          StackTrace = item.StackTrace,
          XmlRequest = item.XmlRequest,
          XmlResponse = item.XmlResponse,
          Login = GetLogin(item.UzId ?? 0, users),
          ConsumedTime = item.ConsumedTime != null ? TimeSpan.FromTicks((long)item.ConsumedTime) : (TimeSpan?)null
        };
      }
    }

    /// <summary>
    /// Zwraca listę wpisów w logu
    /// </summary>
    /// <returns>Lista wpisów w logu</returns>
    //public static ListAfterPagination<LogItem> GetList(LogFiltr filtr)
    //{
    //    using (var dbConnection = new Model.NavireoEntities())
    //    {
    //        var users = dbConnection.pd_Uzytkownik.ToList();

    //        var query = dbConnection.IFx_ApiLog.OrderByDescending(x => x.Date).AsQueryable();

    //        if (filtr.UserId != 0)
    //            query = query.Where(x => x.uz_Id == filtr.UserId);

    //        if (filtr.SelectedTypeList != null && filtr.SelectedTypeList.Where(x => x.Selected).Count() > 0)
    //        {
    //            var typeInt = filtr.SelectedTypeList.Where(x => x.Selected).Select(it => it.TypeId).ToList();
    //            query = query.Where(x => typeInt.Any(y => y == x.Type));
    //        }

    //        var totalRecordCount = query.Count();
    //        if (filtr.Skip != 0)
    //            query = query.Skip(filtr.Skip);
    //        if (filtr.PageSize != 0)
    //            query = query.Take(filtr.PageSize);

    //        var result = new ListAfterPagination<LogItem>(query.ToList().Select(it => new LogItem
    //        {
    //            Id = it.Id,
    //            LogType = (LogType)it.Type,
    //            Date = it.Date != null ? it.Date : new DateTime(),
    //            Message = it.Message,
    //            innerException = it.InnerException,
    //            StackTrace = it.StackTrace,
    //            XmlRequest = it.XmlRequest,
    //            XmlResponse = it.XmlResponse,
    //            Login = GetLogin(it.uz_Id ?? 0, users),
    //            ConsumedTime = it.ConsumedTime != null ? TimeSpan.FromTicks((long)it.ConsumedTime) : (TimeSpan?)null
    //        })
    //            .ToList(), totalRecordCount);

    //        return result;
    //    }
    //}

    /// <summary>
    /// Pobiera użytkowników z logu
    /// </summary>
    /// <returns></returns>
    //public static List<User> GetUsers()
    //{
    //    using (var dbContext = new Model.NavireoEntities())
    //    {
    //        var users = from log in dbContext.IFx_ApiLog
    //                    join pd in dbContext.pd_Uzytkownik on log.uz_Id equals pd.uz_Id
    //                    select pd;

    //        return users.Distinct().Select(it => new User
    //        {
    //            Id = it.uz_Id,
    //            Name = it.uz_Imie,
    //            Surname = it.uz_Nazwisko
    //        }).ToList();
    //    }
    //}


    private static string GetLogin(int id, IEnumerable<Database.PdUzytkownik> users)
    {
      var user = users.FirstOrDefault(x => x.UzId == id);
      if (user != null)
        return user.UzNazwisko + " " + user.UzImie;
      else
        return "";
    }

    private static string Serialize(Type type, object obj)
    {
      try
      {
        using (var stringWriter = new StringWriter())
        {
          var ser = new XmlSerializer(type);
          ser.Serialize(stringWriter, obj);
          return stringWriter.ToString();
        }
      }
      catch (Exception e)
      {
        //Logger.Log(LogType.Exception, e);
        return "Błąd serializacji: " + e.Message;
      }
    }
  }

  public class LogFiltr
  {
    public int UserId { get; set; }
    public List<LogTypItem> SelectedTypeList { get; set; }

    public int PageSize { get; set; }
    public int Skip { get; set; }
  }

  public class LogTypItem
  {
    public int TypeId { get; set; }
    public string Nazwa { get; set; }
    public bool Selected { get; set; }
  }
}
