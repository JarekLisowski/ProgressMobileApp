using Progress.Domain.Api;
using Progress.Domain.Extensions;
using Progress.Domain.Navireo.Api;

namespace Progress.Api
{
  public class NavireoConnector
  {
    string baseUrl = "http://localhost:5270/";


    public async Task<string> UpdateDocument(Document document)
    {
      var httpClient = GetHttpClient();
      var result = await httpClient.PostAsJsonAsync("dokumenty/updateDocument", document, default);
      if (result != null)
      {
        if (result.IsSuccessStatusCode)
        {
          return "Dokument został zapisany";
        }
        var stream = new StreamReader(result.Content.ReadAsStream());
        var data = stream.ReadToEnd();
        Console.WriteLine(data);
      }
      return "Error";
    }


    private HttpClient GetHttpClient()
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(baseUrl + "api/");
      return client; ;
    }
  }
}
