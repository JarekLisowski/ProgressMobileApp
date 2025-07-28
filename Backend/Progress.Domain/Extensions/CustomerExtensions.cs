using Progress.Domain.Api;
using Progress.Domain.Navireo;

namespace Progress.Domain.Extensions
{
  public static class CustomerExtensions
  {
    public static Business ToNavireoCustomer(this Customer customer)
    {
      var busines = new Business
      {
        Id = customer.Id,
        IsNew = customer.Id == 0,
        IsUpdated = customer.Id != 0,
        Active = true,
        HeadquarterAddress = new Location
        {
          City = customer.AdrCity ?? "",
          Code = customer.AdrNip ?? "",
          Country = new Country
          {
            Code = customer.AdrCountryCode ?? "PL"
          },
          Name = customer.AdrName ?? "",
          Street = customer.AdrStreet ?? "",
          Number = customer.AdrStreetNo ?? "",
          VatNumber = customer.AdrNip ?? "",
          ZipCode = customer.AdrZipCode ?? "",
          Email = customer.Email ?? ""
        }
      };

      return busines;
    }
  }
}
