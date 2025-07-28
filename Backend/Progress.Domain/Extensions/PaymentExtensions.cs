using Progress.Domain.Api;
using Progress.Domain.Navireo;

namespace Progress.Domain.Extensions
{
  public static class PaymentExtensions
  {
    public static DocumentSettlement ToNavireoDocumentSettlement(this Payment payment)
    {
      var result = new DocumentSettlement
      {
        IsNew = true,
        RelatedDocumentId = payment.RelatedDocumentId,
        RelatedDocumentNumber = payment.RelatedDocumentNumber,
        Value = payment.Value,
        Buyer = new Location
        {
          Id = payment.PayerId
        }
      };
      return result;
    }
  }
}
