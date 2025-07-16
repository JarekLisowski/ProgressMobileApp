namespace Progress.Domain.Navireo
{
    /// <summary>
    /// Kontrahent
    /// </summary>
    public class Business : BObjectBase
    {
        /// <summary>
        /// Aktywny/Zablokowany
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Dostawca
        /// </summary>
        public bool IsProvider { get; set; }

        /// <summary>
        /// Głowny adres firmy
        /// </summary>
        public Location HeadquarterAddress { get; set; }

        /// <summary>
        /// Lista adresów dostaw
        /// </summary>
        public List<Location> DeliveryAddresses { get; set; }

        /// <summary>
        /// Lista adresów płatników
        /// </summary>
        public List<Location> Payers { get; set; }

        /// <summary>
        /// Lista adresów kupujących
        /// </summary>
        public List<Location> Buyers { get; set; }

        /// <summary>
        /// Data modyfikacji 
        /// </summary>
        public DateTime? ModyficationDate { get; set; }

        /// <summary>
        /// Flaga określa czy kontrahent może mieć dostęp do specjalnych sposobów płatności (np. kredyt kupiecki)
        /// </summary>
        public bool SpecialPayment { get; set; }

        /// <summary>
        /// Termin płatności - dni
        /// </summary>
        public int PaymentDeadline { get; set; }

        /// <summary>
        /// Termin płatności - wystawione faktury/paragony (najstarszy termin)
        /// </summary>
        public int? DuePaymentDeadline { get; set; }

        /// <summary>
        /// Należności do zapłaty
        /// </summary>
        public decimal ToPay { get; set; }

        /// <summary>
        /// Sposób liczenia cen na dokumencie, True - wg. netto, False - brutto
        /// </summary>
        public bool PriceBaseOnNet { get; set; }

        /// <summary>
        /// Id domyślnego poziomu cenowego, jeżeli null to wg. ustawień
        /// </summary>
        public int? PriceLevelId { get; set; }

        /// <summary>
        /// Wzór dla symboli towarów dodawanych przed dostawcę. Np. PS-{0:000000}
        /// </summary>
        public string ProductCodePattern { get; set; }

        /// <summary>
        /// Nr/index dodawanych towarów dla ProductCodePattern
        /// </summary>
        public int ProductIndex { get; set; }


        public Business()
        {
            DeliveryAddresses = new List<Location>();
            Payers = new List<Location>();
            Buyers = new List<Location>();
        }
    }
}
