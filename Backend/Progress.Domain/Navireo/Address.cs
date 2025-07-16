namespace Progress.Domain.Navireo
{
    /// <summary>
    /// Adres
    /// </summary>
    public class Location : BObjectBase
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string GLN { get; set; }

        public string VatNumber { get; set; }

        public string ZipCode { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string Number { get; set; }
        
        public Country Country { get; set; }
        
        public string Telephone { get; set; }

        public string Email { get; set; }

        public string Website { get; set; }

        public string BankAccountNumber { get; set; }
    }
}
