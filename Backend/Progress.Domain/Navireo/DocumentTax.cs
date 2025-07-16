namespace Progress.Domain.Navireo
{
    /// <summary>
    /// Podsumowanie vat
    /// </summary>
    public class DocumentTax : BObjectBase
    {
        /// <summary>
        /// VAT
        /// </summary>
        public Tax Tax { get; set; }

        /// <summary>
        /// Suma netto
        /// </summary>
        public decimal TotalNet { get; set; }

        /// <summary>
        /// Suma brutto
        /// </summary>
        public decimal TotalGross { get; set; }

        /// <summary>
        /// Suma Vat
        /// </summary>
        public decimal TotalTax { get; set; }
    }
}
