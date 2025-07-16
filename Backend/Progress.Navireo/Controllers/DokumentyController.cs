using Microsoft.AspNetCore.Mvc;
using Progress.Domain.Extensions;
using Progress.Navireo.Managers;

namespace Progress.Navireo.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DokumentyController : ControllerBase
  {
    DocumentManager _documentManager;
    public DokumentyController(DocumentManager documentManager)
    {
      _documentManager = documentManager;
    }

    [HttpPost("updateDocument")]
    public object UpdateDocument(Domain.Api.Document request)
    {
      try
      {
        var document = request.ToNavireoDocument();
        document.IsNew = true;
        _documentManager.UpdateDocument(document);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        throw;
      }
      return "OK";
    }
  }
}
