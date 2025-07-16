namespace Progress.Domain.Navireo
{
    /// <summary>
    /// Rozliczenie - dokument kasowy
    /// </summary>
    public class FinanceDocument : BObjectBase
    {
        /// <summary>
        /// Nazwa dokumentu
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Numer dokumentu
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Tytuł
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Data wystawienia
        /// </summary>
        public DateTime IssueDate { get; set; }

        /// <summary>
        /// Dokument którego płatność dotyczy
        /// </summary>
        public CommerceDocumentBase RelatedDocument { get; set; }

        /// <summary>
        /// Wartość brutto
        /// </summary>
        public decimal TotalGross { get; set; }

        /// <summary>
        /// Numer konta - w przypadku przelewu
        /// </summary>
        public string BankAccountNumber { get; set; }

        /// <summary>
        /// Waluta dokumentu
        /// </summary>
        public Currency Currency { get; set; }
        
        /// <summary>
        /// Typ dokumentu
        /// </summary>
        public FinanceDocumentType DocumentType { get; set; }
        
    }
}
