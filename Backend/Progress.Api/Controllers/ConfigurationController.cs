using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Progress.Domain.Api;
using Progress.Domain.Api.Response;

namespace Progress.Api.Controllers
{
//  [Authorize]
  [Route("api/configuration")]
  [ApiController]
  public class ConfigurationController : ApiControllerBase
  {

    IServiceProvider _serviceProvider;
    BusinessLogic.ConfigurationManager _configurationManager;
    IMapper _mapper;

    public ConfigurationController(IServiceProvider serviceProvider, IMapper mapper, BusinessLogic.ConfigurationManager configurationManager) 
      : base(serviceProvider)
    {
      _serviceProvider = serviceProvider;
      _configurationManager = configurationManager;
      _mapper = mapper;
    }

    [HttpGet("paymentMethods")]
    public PaymentMethodsResponse GetPaymentMethods()
    {
      var data = _configurationManager.GetPaymentMethods();
      return new PaymentMethodsResponse
      {
        Data = _mapper.Map<PaymentMethod[]>(data)
      };
    }

    [HttpGet("deliveryMethods")]
    public DeliveryMethodsResponse GetDeliveryMethods()
    {
      var data = _configurationManager.GetDeliveryMethods();
      return new DeliveryMethodsResponse
      {
        Data = _mapper.Map<DeliveryMethod[]>(data)
      };
    }
  }
}
