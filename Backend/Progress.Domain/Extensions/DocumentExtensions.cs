using Progress.Domain.Api;
using Progress.Domain.Navireo;

namespace Progress.Domain.Extensions
{
  public static class DocumentExtensions
  {
    public static CommerceDocumentBase ToNavireoDocument(this Document document)
    {
      var documentType = DocumentEnum.CustomerOrder;
      switch (document.DocumentType)
      {
        case "Invoice": documentType = DocumentEnum.SalesInvoice; break;
        case "Order": documentType = DocumentEnum.CustomerOrder; break;
        case "Order internal": documentType = DocumentEnum.StoreOrder; break;
      }

      var navDocument = new CommerceDocumentBase
      {
        DocumentType = documentType,
        BuyerAddress = new Location
        {
          Id = document.CustomerId
        },
        Comment = document.Comment,
        PaidCashGross = document.CashPayment,
        Payment = new PaymentType
        {
          Id = document.SecondPaymentMethod ?? 0
        },
        PaymentPaidGross = document.SecondPaymentAmount,
        DocumentItems = document.Items.Select(it => new Navireo.DocumentItem
        {
          Product = new Navireo.Product
          { 
            Id = it.ProductId,
          },
          Amount = it.Quantity,
          PriceGross = it.PriceGross,
          PriceNet = it.PriceNet,
          DiscountRate = it.DiscountRate,
          Tax = new Tax
          {
            Rate = it.TaxRate,
          },
          IsNew = true
        }).ToList(),
        Delivery = new DeliveryType
        {
          Id = document.DeliveryMethod ?? 0,
        },
        UserId = document.UserId ?? 0
      };
      return navDocument;
    }
  }
}
