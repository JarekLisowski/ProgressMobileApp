using Progress.Domain.Navireo;

namespace Progress.Domain.Extensions
{
  public static class DocumentEnumExtension
  {
    public static string GetName(this DocumentEnum doc)
    {
      switch (doc)
      {
        case DocumentEnum.SalesInvoice: return "Invoice";
        case DocumentEnum.CustomerOrder: return "Order";
        case DocumentEnum.StoreOrder: return "Internal order";
      }
      return "";
    }
  }
}
