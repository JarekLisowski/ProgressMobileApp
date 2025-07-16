namespace Progress.Domain.Navireo
{
    public class Product : BObjectBase
    {
        /// <summary>
        /// Flaga oznaczająca czy zmienił się stan towaru
        /// </summary>
        public bool StockChanged { get; set; }
        /// <summary>
        /// Czy aktywny
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Flaga - wyprzedaż
        /// </summary>
        public bool Sale { get; set; }

        /// <summary>
        /// Url produktu
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Symbol
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Symbol u dostawcy
        /// </summary>
        public string ProviderCode { get; set; }

        /// <summary>
        /// Kod kreskowy
        /// </summary>
        public string EAN { get; set; }

        /// <summary>
        /// Opis
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Jednostka sprzedaży
        /// </summary>
        public string Unit { get; set; }

        
        /// <summary>
        /// Vat
        /// </summary>
        public Tax Tax { get; set; }

        /// <summary>
        /// Vat - do delete
        /// </summary>
        public decimal TaxRate { get; set; }

        /// <summary>
        /// Dostępność
        /// </summary>
        public decimal? Availability { get; set; }

        public decimal? Availability2 { get; set; }

        /// <summary>
        /// Czy ilość zależy od wariantów
        /// </summary>
        public bool StockDependOnVariants { get; set; }

        /// <summary>
        /// Powiadom o dostępności produktu
        /// </summary>
        public bool NotifyWhenAvailable { get; set; }

        /// <summary>
        /// Cena netto
        /// </summary>
        public decimal PriceNet { get; set; }

        /// <summary>
        /// Cena brutto
        /// </summary>
        public decimal PriceGross { get; set; }

        /// <summary>
        /// Waluta
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// Data modyfikacji
        /// </summary>
        public DateTime? ModyficationDate { get; set; }

        /// <summary>
        /// Id grupy towarowej
        /// </summary>
        public int? ProductGroupId { get; set; }

        /// <summary>
        /// Czas dostawy - dni
        /// </summary>
        public int DeliveryTime { get; set; }

        /// <summary>
        /// Ilość w paczce
        /// </summary>
        public decimal PackageQuantity { get; set; }

        /// <summary>
        /// PKWiU
        /// </summary>
        public string PKWiU { get; set; }

        /// <summary>
        /// Waga
        /// </summary>
        public decimal? Weight { get; set; }


    }
}
