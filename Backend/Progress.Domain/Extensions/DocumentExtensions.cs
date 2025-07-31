using Progress.Domain.Navireo;

namespace Progress.Domain.Extensions
{
  public static class DocumentExtensions
  {
    public static CommerceDocumentBase ToNavireoDocument(this Api.Document document)
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
        IssueDate = DateTime.Today,
        DocumentType = documentType,
        BuyerAddress = new Location
        {
          Id = document.CustomerId
        },
        Comment = document.Comment,
        PaidCashGross = document.CashPayment,
        Payment = ((document.SecondPaymentMethod ?? 0) != 0) ? new PaymentType
        {
          Id = document.SecondPaymentMethod ?? 0
        } : null,
        PaymentPaidGross = document.SecondPaymentAmount,
        PaymentDeadline = document.IssueDate.AddDays(document.PaymentDueDays),
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
          IsNew = true,
          DiscountFamilyId = it.PromoSetId ?? 0,
          DiscountItemId = it.PromoItemId ?? 0
        }).ToList(),
        Delivery = new DeliveryType
        {
          Id = document.DeliveryMethod ?? 0,
          IsNew = true
        },
        UserId = document.UserId ?? 0
      };
      return navDocument;
    }

    public static VatLine[] GetVatSummary(this Model.Document document)
    {
      var vatGroups = document.Items.GroupBy(it => it.TaxRate);
      var result = vatGroups.Select(it => new VatLine
      {
        Rate = it.Key,
        Net = it.Sum(it => it.LineNet),
      }).ToArray();
      foreach (var vatGroup in result)
      {
        vatGroup.Net = Math.Round(vatGroup.Net, 2);
        vatGroup.Gross = Math.Round(vatGroup.Net * (1 + vatGroup.Rate / 100), 2);
        vatGroup.Tax = vatGroup.Gross - vatGroup.Net;
        vatGroup.RateName = $"{vatGroup.Rate:N1}%";
      }
      return result;
    }

    public static void CalculateVatSummary(this Model.Document document)
    {
      document.VatSummary = document.GetVatSummary();
      document.TotalTax = document.TotalGross - document.TotalNet;
    }

  }
}
