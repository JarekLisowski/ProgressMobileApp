namespace Progress.Domain.Navireo
{
    /// <summary>
    /// Cena / Poziom cenowy
    /// </summary>
    public class Price : BObjectBase
    {
        /// <summary>
        /// Cena główna/podstawowa
        /// </summary>
        public bool Primary { get; set; }

        /// <summary>
        /// Czy cena promocyjna
        /// </summary>
        public bool PromoPrice { get; set; }

        /// <summary>
        /// Nazwa
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Opis
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Wartość netto
        /// </summary>
        public decimal ValueNet { get; set; }

        /// <summary>
        /// Wartość brutto
        /// </summary>
        public decimal ValueGross { get; set; }

        /// <summary>
        /// Vat
        /// </summary>
        public Tax Tax { get; set; }

        /// <summary>
        /// Waluta
        /// </summary>
        public Currency Curency { get; set; }
    }
}
