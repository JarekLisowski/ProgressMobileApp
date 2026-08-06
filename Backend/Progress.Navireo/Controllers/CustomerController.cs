using Microsoft.AspNetCore.Mvc;
using Progress.Domain.Api.Request;
using Progress.Domain.Api.Response;
using Progress.Domain.Extensions;
using Progress.Domain.Navireo;
using Progress.Navireo.Managers;

namespace Progress.Navireo.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class CustomerController : ControllerBase
  {
    CustomerManager _customerManager;

    public CustomerController(CustomerManager customerManager)
    {
      _customerManager = customerManager;
    }

    [HttpPost("save")]
    public SaveCustomerResponse SaveCustomer(UpdateCustomerRequest request)
    {
      try
      {
        var customer = request.Customer.ToNavireoCustomer();
        if (customer != null)
        {
          var result = _customerManager.UpdateCustomer(request.OperatorId, customer);
          return new SaveCustomerResponse
          {
            CustomerId = result ?? 0,
            IsError = result == 0
          };
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return new SaveCustomerResponse
        {          
          IsError = true,
          Message = ex.Message
        };
      }
      return new SaveCustomerResponse
      {
        IsError = true
      };
    }
  }
}

