using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Progress.Database;
using Progress.Domain.Model;

namespace Progress.Infrastructure.Database.Repository
{
  public class CommonRepository : DatabaseContext
  {

    IDatabaseRepository<PaymentMethod, IfxApiFormaPlatnosci> paymentMethodRepository;
    IDatabaseRepository<DeliveryMethod, IfxApiSposobDostawy> deliveryMethodRepository;

    public CommonRepository(
      DbContext dbContext, 
      IConfigurationProvider automapperConfiguration,
      IDatabaseRepository<PaymentMethod, IfxApiFormaPlatnosci> paymentMethodRepo,
      IDatabaseRepository<DeliveryMethod, IfxApiSposobDostawy> deliveryMethodRepo
      )
      : base(dbContext, automapperConfiguration)
    { 
      paymentMethodRepository = paymentMethodRepo;
      deliveryMethodRepository = deliveryMethodRepo;
    }

    public IEnumerable<PaymentMethod> GetPaymentMethods()
    {
      return paymentMethodRepository.SelectWhere(it => true).OrderBy(it => it.Name).ToArray();
    }

    public IEnumerable<DeliveryMethod> GetDeliverMethods()
    {
      return deliveryMethodRepository.SelectWhere(it => true).OrderBy(it => it.Name).ToArray();
    }


  }
}
