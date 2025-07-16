using Progress.Database;
using Progress.Domain.Model;
using Progress.Infrastructure.Database.Repository;

namespace Progress.BusinessLogic
{
  public class ConfigurationManager
  {
    private IDatabaseRepository<PaymentMethod, IfxApiFormaPlatnosci> _paymentMethodRepository;
    private IDatabaseRepository<DeliveryMethod, IfxApiSposobDostawy> _deliveryMethodRepository;

    public ConfigurationManager(
      IDatabaseRepository<PaymentMethod, IfxApiFormaPlatnosci> paymentMethodRepository,
      IDatabaseRepository<DeliveryMethod, IfxApiSposobDostawy> deliveryMethodRepository)
    {
      _deliveryMethodRepository = deliveryMethodRepository;
      _paymentMethodRepository = paymentMethodRepository;
    }

    public IEnumerable<PaymentMethod> GetPaymentMethods()
    {
      return _paymentMethodRepository.SelectWhere(it => it.Aktywna).OrderBy(it => it.Name).ToArray();
    }

    public IEnumerable<DeliveryMethod> GetDeliveryMethods()
    {
      return _deliveryMethodRepository.SelectWhere(it => true).OrderBy(it => it.Name).ToArray();
    }

  }
}
