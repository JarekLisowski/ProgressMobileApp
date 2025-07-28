using Progress.Domain.Api;
using Progress.Domain.Api.Request;
using Progress.Domain.Api.Response;

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

    internal async Task<string> UpdateOrAddCustomer(Customer customer, int userId)
    {
      var httpClient = GetHttpClient();
      var request = new UpdateCustomerRequest
      { 
        OperatorId = userId,
        Customer = customer
      };
      
      var result = await httpClient.PostAsJsonAsync("customer/update", request, default);
      if (result != null)
      {
        if (result.IsSuccessStatusCode)
        {
          return "Kontrahent został zapisany";
        }
        var stream = new StreamReader(result.Content.ReadAsStream());
        var data = stream.ReadToEnd();
        Console.WriteLine(data);
      }
      return "Error";
    }

    internal async Task<ApiResult> AddPayment(Payment payment, int userId)
    {
      var httpClient = GetHttpClient();
      var request = new PaymentRequest
      {
        OperatorId = userId,
        Payment = payment
      };

      var result = await httpClient.PostAsJsonAsync("document/pay", request, default);
      if (result != null)
      {
        if (result.IsSuccessStatusCode)
        {
          var apiResult = await result.Content.ReadFromJsonAsync<ApiResult>();
          if (apiResult != null) 
            return apiResult;
          return new ApiResult
          {
            IsError = true,
            Message = "Nieznany błąd: brak danych"
          };
        }
        var stream = new StreamReader(result.Content.ReadAsStream());
        var data = stream.ReadToEnd();
        Console.WriteLine(data);
        return new ApiResult
        {
          IsError = true,
          Message = $"{result.StatusCode}: {result.ReasonPhrase}"
        };
      }
      return new ApiResult
      { 
        IsError = true,
        Message = "Nieznany błąd"      
      };

    }

    private HttpClient GetHttpClient()
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(baseUrl + "api/");
      return client; ;
    }
  }
}
