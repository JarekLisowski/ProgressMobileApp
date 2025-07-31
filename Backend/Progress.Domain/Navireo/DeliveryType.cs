namespace Progress.Domain.Navireo
{
    /// <summary>
    /// Sposób dostawy
    /// </summary>
    public class DeliveryType : BObjectBase
    {
        /// <summary>
        /// Identyfikator
        /// </summary>
        public string Identity { get; set; }

        /// <summary>
        /// Flaga czy aktywna
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Ilość paczek
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Nazwa
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Symbol
        /// </summary>
        public string Code { get; set; }

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
        /// VAT
        /// </summary>
        public Tax? Tax { get; set; }

        /// <summary>
        /// Max waga towarów dla dostawy
        /// </summary>
        public decimal MaxWeight { get; set; }

        /// <summary>
        /// Płatności powiązane z dostawą
        /// </summary>
        public List<PaymentType> RelatedPayments { get; set; }

        public decimal TaxValue { get; set; }
    }
}
