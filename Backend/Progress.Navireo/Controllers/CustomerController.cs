using Microsoft.AspNetCore.Mvc;
using Progress.Domain.Api.Request;
using Progress.Domain.Extensions;
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

    [HttpPost("update")]
    public object UpdateCustomer(UpdateCustomerRequest request)
    {
      try
      {
        var customer = request.Customer.ToNavireoCustomer();
        if (customer != null)
        {
          _customerManager.UpdateCustomer(request.OperatorId, customer);
        }
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

