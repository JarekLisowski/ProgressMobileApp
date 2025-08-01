﻿using Progress.Domain.Api;
using Progress.Domain.Api.Request;
using Progress.Domain.Api.Response;

namespace Progress.Api
{
  public class NavireoConnector
  {
    string baseUrl = "http://localhost:5270/";


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
      }
      return new SaveDocumentResponse
      {
        IsError = true,
        Message = "Wystąpił błąd."
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

    internal async Task<ApiResult> AddPayment(Payment payment, int userId)
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
          Message = $"{(int)result.StatusCode}: {result.ReasonPhrase}"
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
