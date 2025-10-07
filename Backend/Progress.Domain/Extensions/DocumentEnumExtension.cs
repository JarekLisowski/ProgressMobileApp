using Progress.Domain.Navireo;

namespace Progress.Domain.Extensions
{
  public static class DocumentEnumExtension
  {
    public static string GetName(this DocumentEnum doc)
    {
      switch (doc)
      {
        case DocumentEnum.SalesInvoice: return "Faktura";
        case DocumentEnum.CustomerOrder: return "Zamówienie";
        case DocumentEnum.StoreOrder: return "Zamówienie wenętrzne";
        case DocumentEnum.Receipt: return "Paragon";
      }
      return "";
    }
  }
}
