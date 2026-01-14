using Progress.Domain.Api;
using Progress.Domain.Api.Request;
using Progress.Domain.Api.Response;

namespace Progress.Api
{
  public class NavireoConnector
  {
    string baseUrl = "";// "http://localhost:5270/";

    public NavireoConnector(IConfiguration configurationProvider)
    {
      var url = configurationProvider.GetValue<string>("NavireoApi");
      if (url != null) 
        baseUrl = url;
    }

    public async Task<SaveDocumentResponse> SaveDocument(Document document)
    {
      var httpClient = GetHttpClient();
      var result = await httpClient.PostAsJsonAsync("dokumenty/saveDocument", document, default);
      if (result != null)
      {
        if (result.IsSuccessStatusCode)
        {
          var navireoResult = await result.Content.ReadFromJsonAsync<SaveDocumentResponse>();
          if (navireoResult != null)           
            return navireoResult;
        }
        var stream = new StreamReader(result.Content.ReadAsStream());
        var data = stream.ReadToEnd();
        Console.WriteLine(data);
        return new SaveDocumentResponse
        {
          IsError = true,
          Message = $"Wystąpił błąd połączenia z Navireo. Serwer zwrócił status: {result.StatusCode}. {data}"
        };
      }
      return new SaveDocumentResponse
      {
        IsError = true,
        Message = "Wystąpił nieznany błąd połączenia z Navireo."
      };
    }

    public async Task<SaleSummaryResponse> GetSaleSummary(int operatorId, DateTime dateFrom, DateTime dateTo, string currencyCode = "PLN")
    {
      var httpClient = GetHttpClient();
      var request = new SaleSummaryRequest
      {
        CurrencyCode = currencyCode,
        OperatorId = operatorId,
        DateFrom = dateFrom,
        DateTo = dateTo,
      };
      var result = await httpClient.PostAsJsonAsync("finance/getSaleSummary", request, default);
      if (result != null)
      {
        if (result.IsSuccessStatusCode)
        {
          var navireoResult = await result.Content.ReadFromJsonAsync<SaleSummaryResponse>();
          if (navireoResult != null)
            return navireoResult;
        }
        var stream = new StreamReader(result.Content.ReadAsStream());
        var data = stream.ReadToEnd();
        Console.WriteLine(data);
        return new SaleSummaryResponse
        {
          IsError = true,
          Message = $"Wystąpił błąd połączenia z Navireo. Serwer zwrócił status: {result.StatusCode}. {data}"
        };
      }
      return new SaleSummaryResponse
      {
        IsError = true,
        Message = "Wystąpił nieznany błąd połączenia z Navireo."
      };
    }

    internal async Task<string> SaveCustomer(Customer customer, int userId)
    {
      var httpClient = GetHttpClient();
      var request = new UpdateCustomerRequest
      { 
        OperatorId = userId,
        Customer = customer
      };
      
      var result = await httpClient.PostAsJsonAsync("customer/save", request, default);
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

    internal async Task<SaveDocumentResponse> AddPayment(Payment payment, int userId)
    {
      var httpClient = GetHttpClient();
      var request = new PaymentRequest
      {
        OperatorId = userId,
        Payment = payment
      };

      var result = await httpClient.PostAsJsonAsync("dokumenty/pay", request, default);
      if (result != null)
      {
        if (result.IsSuccessStatusCode)
        {
          var apiResult = await result.Content.ReadFromJsonAsync<SaveDocumentResponse>();
          if (apiResult != null) 
            return apiResult;
          return new SaveDocumentResponse
          {
            IsError = true,
            Message = "Nieznany błąd: brak danych"
          };
        }
        var stream = new StreamReader(result.Content.ReadAsStream());
        var data = stream.ReadToEnd();
        Console.WriteLine(data);
        return new SaveDocumentResponse
        {
          IsError = true,
          Message = $"{(int)result.StatusCode}: {result.ReasonPhrase}"
        };
      }
      return new SaveDocumentResponse
      { 
        IsError = true,
        Message = "Nieznany błąd"      
      };

    }

    private HttpClient GetHttpClient()
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(baseUrl + "/api/");
      return client; ;
    }
  }
}
