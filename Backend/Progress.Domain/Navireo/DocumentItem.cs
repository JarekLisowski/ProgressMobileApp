namespace Progress.Domain.Navireo
{
    /// <summary>
    /// Pozycja dokumentu
    /// </summary>
    public class DocumentItem : BObjectBase
    {
        /// <summary>
        /// Towar
        /// </summary>
        public Product Product { get; set; }

        /// <summary>
        /// Ilość
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Rabat/Obniżka %
        /// </summary>
        public decimal DiscountRate { get; set; }

        /// <summary>
        /// Cena netto
        /// </summary>
        public decimal PriceNet { get; set; }

        /// <summary>
        /// Cena netto po rabacie
        /// </summary>
        public decimal PriceNetAfterDiscount { get; set; }

        /// <summary>
        /// Cena brutto
        /// </summary>
        public decimal PriceGross { get; set; }

        /// <summary>
        /// Cena brutto po rabacie
        /// </summary>
        public decimal PriceGrossAfterDiscount { get; set; }

        /// <summary>
        /// Suma netto
        /// </summary>
        public decimal TotalNet { get; set; }

        /// <summary>
        /// Suma netto po rabacie 
        /// </summary>
        public decimal TotalNetAfterDiscount { get; set; }

        /// <summary>
        /// Suma brutto
        /// </summary>
        public decimal TotalGross { get; set; }

        /// <summary>
        /// Suma brutto po rabacie
        /// </summary>
        public decimal TotalGrossAfterDiscount { get; set; }

        /// <summary>
        /// Suma Vat
        /// </summary>
        public decimal TotalTax { get; set; }

        /// <summary>
        /// Suma Vat po rabacie
        /// </summary>
        public decimal TotalTaxAfterDiscount { get; set; }

        /// <summary>
        /// Vat
        /// </summary>
        public Tax Tax { get; set; }

        /// <summary>
        /// Wariant towaru (kolor, rozmiar itp. itd.)
        /// </summary>
        public string ProductVariant { get; set; }

        /// <summary>
        /// Id pozycji z zestawu promocyjnego
        /// </summary>
        public int DiscountItemId { get; set; }

        /// <summary>
        /// Id pozycji z zestawu promocyjnego - z urządzeń
        /// </summary>
        public int DiscountFamilyId { get; set; }

        /// <summary>
        /// Flaga czy pozycja jest z zestawu promocyjnego
        /// </summary>
        public bool DiscountSetItem { get; set; }

        /// <summary>
        /// Id pozycji powiązanej, dotyczy korekt FS i PA
        /// </summary>
        public int? RelatedItemId { get; set; }
    }
}
