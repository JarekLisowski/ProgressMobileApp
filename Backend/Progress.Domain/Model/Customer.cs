namespace Progress.Domain.Model;

public partial class Customer
{
    public int Id { get; set; }

    public string? Code { get; set; }

    public string? Regon { get; set; }

    public string? Email { get; set; }

    public int? IdOpiekun { get; set; }

    public string? AdrName { get; set; }

    public string? AdrNameFull { get; set; }

    public string? AdrNip { get; set; }

    public string? AdrTel { get; set; }

    public string? AdrStreet { get; set; }

    public string? AdrStreetNo { get; set; }

    public string? AdrNumber { get; set; }

    public string? AdrZipCode { get; set; }

    public string? AdrCity { get; set; }

    public int? AdrCountryId { get; set; }

    public string? AdrCountry { get; set; }

    public string? AdrCountryCode { get; set; }

    public string? DelivName { get; set; }

    public string? DelivCode { get; set; }

    public string? DelivNip { get; set; }

    public string? DelivTel { get; set; }

    public string? DelivStreet { get; set; }

    public string? DelivStreetNo { get; set; }

    public string? DelivNumber { get; set; }

    public string? DelivZipCode { get; set; }

    public string? DelivCity { get; set; }

    public int? DelivCountryId { get; set; }

    public string? DelivCountry { get; set; }

    public string? DelivCountryCode { get; set; }

    public bool? Blocked { get; set; }

    public DateTime? ChangeDate { get; set; }

    public bool? OneTime { get; set; }

    public string? CustEmploee { get; set; }

    public bool? PayTermin { get; set; }

    public int? PayDays { get; set; }

    public string? www { get; set; }

    public int? PriceId { get; set; }

}
