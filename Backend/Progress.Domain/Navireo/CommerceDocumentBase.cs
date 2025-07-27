namespace Progress.Domain.Navireo
{
    /// <summary>
    /// Dokument
    /// </summary>
    public class CommerceDocumentBase : DocumentBase
    {        
        /// <summary>
        /// Typ dokumentu
        /// </summary>
        public DocumentEnum DocumentType { get; set; }

        /// <summary>
        /// Pozycje dokumentu
        /// </summary>
        public List<DocumentItem> DocumentItems { get; set; }

        public decimal ItemsTotalNet
        {
            get
            {
                if (DocumentItems != null)
                    return DocumentItems.Sum(x => x.TotalNet);
                else
                    return 0;
            }
        }

        public decimal ItemsTotalGross
        {
            get
            {
                if (DocumentItems != null)
                    return DocumentItems.Sum(x => x.TotalGross);
                else
                    return 0;
            }
        }

        /// <summary>
        /// Flaga - czy dok. zrealizowany
        /// </summary>
        public bool Completed { get; set; }

        /// <summary>
        /// Flaga - czy dok. został anulowany
        /// </summary>
        public bool Cancelled { get; set; }

        /// <summary>
        /// Kontrahent
        /// </summary>
        public Business Business { get; set; }

        /// <summary>
        /// Adres zamawiającego - historyczny, w chwili zapisu dokumentu
        /// </summary>
        public Location OrderingAddress { get; set; }
        
        /// <summary>
        /// Adres dostawy - w chwili zapisu dokumentu
        /// </summary>
        public Location DeliveryAddress { get; set; }
        
        /// <summary>
        /// Adres płatnika - historyczny, w chwili zapisu dokumentu
        /// </summary>
        public Location PayerAddress { get; set; }
        
        /// <summary>
        /// Sposób płatności
        /// </summary>
        public PaymentType? Payment { get; set; }

        /// <summary>
        /// Rodzaj dostawy
        /// </summary>
        public DeliveryType Delivery { get; set; }

        /// <summary>
        /// Podsumowanie Vat
        /// </summary>
        public List<DocumentTax> DocumentTax { get; set; }

        /// <summary>
        /// Powiązany KP - numer
        /// </summary>
        public int SettlementNumber { get; set; }

        /// <summary>
        /// Numer pełny KP
        /// </summary>
        public string SettlementFullNumber { get; set; }

        /// <summary>
        /// Czy wyeksportowany do zewnętrznego ERP
        /// </summary>
        public bool Exported { get; set; }

        /// <summary>
        /// Kiedy wyeksportowany - system zewnętrzny
        /// </summary>
        public DateTime? ExportDate { get; set; }

        /// <summary>
        /// Błąd exportu
        /// </summary>
        public string ExportException { get; set; }

        /// <summary>
        /// Rozliczenia dokumentu
        /// </summary>
        public List<FinanceDocument> SettlementList { get; set; }

    }
}
