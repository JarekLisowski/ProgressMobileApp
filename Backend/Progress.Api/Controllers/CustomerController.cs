using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Progress.BusinessLogic;
using Progress.Domain.Api;
using Progress.Domain.Api.Response;

namespace Progress.Api.Controllers
{
  [Authorize]
  [Route("api/customer")]
  [ApiController]
  public class CustomerController : ApiControllerBase
  {
    CustomerManager _customerManager;
    IMapper _mapper;

    public CustomerController(CustomerManager customerManager, IMapper autoMapper, IServiceProvider serviceProvider)
      : base(serviceProvider)
    {
      _customerManager = customerManager;
      _mapper = autoMapper;
    }

    [HttpGet("{id}")]
    public CustomerResponse Get(int id)
    {
      var data = _customerManager.Get(id);
      if (data != null)
      {
        return new CustomerResponse
        {
          Data = _mapper.Map<Customer>(data)
        };
      }
      return new CustomerResponse
      {
        IsError = true,
        Message = "No customer"
      };

    }

    [HttpGet("search/{pattern}")]
    public CustomerListResponse Search(string pattern, int limit = 50)
    {
      var data = _customerManager.Search(pattern, GetUser()?.CechaId, limit);
      if (data != null)
      {
        var test = _mapper.Map<Customer[]>(data);
        return new CustomerListResponse
        {
          Data = _mapper.Map<Customer[]>(data)
        };
      }
      return new CustomerListResponse();
    }

    [HttpPost]
    public string Create()
    {
      return "Not implemented";
    }

    [HttpPut]
    public string Update()
    {
      return "Not implemented";
    }

  }
}