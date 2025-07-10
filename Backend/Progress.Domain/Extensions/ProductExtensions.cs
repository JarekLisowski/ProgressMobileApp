using Progress.Domain.Model;

namespace Progress.Domain.Extensions
{
  public static class ProductExtensions
  {
    public static void SetupUserPrices(this Product product, User user)
    {
      product.Price = product.Prices[user.DefaultPrice];
      Dictionary<int, Price> availablePrices = new Dictionary<int, Price>();
      foreach (var item in product.Prices)
      {
        var userPrice = user.PriceLevelList.FirstOrDefault(it => it.Id == item.Key);
        if (userPrice != null)
        {
          item.Value.Name = userPrice.Name;
          availablePrices.Add(item.Key, item.Value);
        }
      }
      product.Prices = availablePrices;
    }
  }
}
