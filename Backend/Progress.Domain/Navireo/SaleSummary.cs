namespace Progress.Domain.Navireo
{
    /// <summary>
    /// Podsumowanie sprzedaży
    /// </summary>
    public class SaleSummary
    {
        /// <summary>
        /// Aktualny stan kasy
        /// </summary>
        public decimal CurrentCash { get; set; }

        /// <summary>
        /// Sprzedaż netto (paragony i faktury)
        /// </summary>
        public decimal SaleNet { get; set; }

        /// <summary>
        /// Sprzedaż brutto (paragony i faktury)
        /// </summary>
        public decimal SaleGross { get; set; }

        /// <summary>
        /// Sprzedaż w gotówce
        /// </summary>
        public decimal SaleCashGross { get; set; }

        /// <summary>
        /// Suma przyjętej gotówki
        /// </summary>
        public decimal ReceivedCash { get; set; }

        /// <summary>
        /// Lista typów dokumentów wraz z podsumowaniem
        /// </summary>
        public List<DocumentSummary> DocumentSummary { get; set; }
        
        
        public SaleSummary()
        {
            DocumentSummary = new List<DocumentSummary>();
        }
    }

    /// <summary>
    /// Podsumowanie dla danego typu dokumentu
    /// </summary>
    public class DocumentSummary
    {
        /// <summary>
        /// TypDokumentu
        /// </summary>
        public DocumentEnum Type { get; set; }

        /// <summary>
        /// Ilość dokumentów
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Suma netto
        /// </summary>
        public decimal TotalNet { get; set; }

        /// <summary>
        /// Suma brutto
        /// </summary>
        public decimal TotalGross { get; set; }

        /// <summary>
        /// Suma zapłacona w gotówce
        /// </summary>
        public decimal TotalCashGross { get; set; }

    }
}
