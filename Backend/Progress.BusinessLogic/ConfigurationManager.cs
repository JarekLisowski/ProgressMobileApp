using Microsoft.EntityFrameworkCore;
using Progress.Database;
using Progress.Domain.Model;
using Progress.Infrastructure.Database.Repository;
using System.Security.Cryptography;

namespace Progress.BusinessLogic
{
  public class ConfigurationManager
  {
    private IDatabaseRepository<PaymentMethod, IfxApiFormaPlatnosci> _paymentMethodRepository;
    private IDatabaseRepository<DeliveryMethod, IfxApiSposobDostawy> _deliveryMethodRepository;
    private NavireoDbContext _dbContext;
    public ConfigurationManager(
      IDatabaseRepository<PaymentMethod, IfxApiFormaPlatnosci> paymentMethodRepository,
      IDatabaseRepository<DeliveryMethod, IfxApiSposobDostawy> deliveryMethodRepository,
      NavireoDbContext dbContext
      )
    {
      _dbContext = dbContext;
      _deliveryMethodRepository = deliveryMethodRepository;
      _paymentMethodRepository = paymentMethodRepository;
    }

    public IEnumerable<PaymentMethod> GetPaymentMethods()
    {      
      return _paymentMethodRepository.SelectWhere(it => it.Aktywna).OrderBy(it => it.Name).ToArray();
    }

    public IEnumerable<DeliveryMethod> GetDeliveryMethods()
    {
      var data = from delivery in _dbContext.IfxApiSposobDostawies.AsNoTracking()
                 join towar in _dbContext.TwTowars.AsNoTracking() on delivery.TwId equals towar.TwId
                 join cena in _dbContext.TwCenas.AsNoTracking() on towar.TwId equals cena.TcIdTowar
                 join vat in _dbContext.SlStawkaVats.AsNoTracking() on towar.TwIdVatSp equals vat.VatId
                 where(delivery.Aktywny)
                 select new DeliveryMethod
                 {
                   Id = delivery.Id,
                   Name = delivery.Nazwa,
                   TwId = delivery.TwId,
                   PriceGross = cena.TcCenaBrutto1 ?? 0,
                   PriceNet = cena.TcCenaNetto1 ?? 0,
                   TaxRate = vat.VatStawka,
                   MaxValue = delivery.MaxWartosc,
                   MinValue = delivery.MinWartosc
                 };

      return data.ToArray();
    }

  }
}
