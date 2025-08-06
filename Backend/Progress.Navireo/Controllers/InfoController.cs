using Microsoft.AspNetCore.Mvc;
using Progress.Navireo.Domain;
using Progress.Navireo.Navireo;

namespace Progress.Navireo.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class InfoController : Controller
  {
    NavireoApplication _navireoApplication;
    public InfoController(NavireoApplication navireoApplication)
    {
        _navireoApplication = navireoApplication;
    }

    [HttpGet]
    public InfoResponse Info()
    {
      string navireo = "";
      try
      {
        var app = _navireoApplication.GetNavireo();
        navireo = app?.Wersja ?? "not running";
      }
      catch (Exception ex) 
      {
        navireo = ex.Message;
      }
      return new InfoResponse
      {
        Navireo = navireo,
      };
    }
  }
}
