namespace Progress.Domain.Navireo
{
    public enum DocumentEnum
    {
        /// <summary>
        /// Zamówienie od klienta
        /// </summary>
        CustomerOrder = 1,

        /// <summary>
        /// Zamowienie do dostawcy
        /// </summary>
        ProviderOrder = 2,

        /// <summary>
        /// Paragon
        /// </summary>
        Receipt = 3,

        /// <summary>
        /// Faktura sprzedaży
        /// </summary>
        SalesInvoice = 4,

        /// <summary>
        /// Faktura zakupu
        /// </summary>
        PurchaseInvoice = 5,

        /// <summary>
        /// Korekta faktury zakupu
        /// </summary>
        PurchaseInvoiceCorrection = 6,

        /// <summary>
        /// Korekta faktury sprzedaży
        /// </summary>
        SalesInvoiceCorrection = 7,

        /// <summary>
        /// Zapytanie do dostawcy
        /// </summary>
        ProviderInquiry = 8,

        /// <summary>
        /// Zapytanie od klienta
        /// </summary>
        CustomerInquiry = 9,

        /// <summary>
        /// Oferta od dostawcy
        /// </summary>
        ProviderOffer = 10,

        /// <summary>
        /// Oferta dla klienta
        /// </summary>
        CustomerOffer = 11,

        /// <summary>
        /// Reklamacja
        /// </summary>
        Complaint = 12,

        /// <summary>
        /// Nota przewozowa przychodząca
        /// </summary>
        ConsigmentNoteIncoming = 13,

        /// <summary>
        /// Nota przewozowa wychodząca
        /// </summary>
        ConsigmentNoteOutcoming = 14,

        /// <summary>
        /// Zamówienie magazynowe
        /// </summary>
        StoreOrder = 15,

        /// <summary>
        /// Dokument kasowy
        /// </summary>
        DocumentPayment = 16,

        /// <summary>
        /// Rozliczenie dokumentu
        /// </summary>
        DocumentSettlement = 17,

        /// <summary>
        /// Faktura ProForma
        /// </summary>
        ProFormaInvoice = 18,
        ProductCatalogue = 19
    }

    public enum FinanceDocumentType
    {
        KasaPrzyjmie = 1,
        KasaWyda = 2,
        WplataBankowa = 3,
        WyplataBankowa = 4
    }

    /// <summary>
    /// Typ adresu
    /// </summary>
    public enum AddressType
    {
        /// <summary>
        /// Adres główny kontrahenta
        /// </summary>
        HeadquarterAddress = 1,

        /// <summary>
        /// Adres kupującego (na fakturę)
        /// </summary>
        Buyer = 2,

        /// <summary>
        /// Adres płatnika
        /// </summary>
        Payer = 3,

        /// <summary>
        /// Adres dostawy
        /// </summary>
        Delivery = 4,

        /// <summary>
        /// Adres zamawiającego
        /// </summary>
        Order = 5
    }

    public enum ConsignmentNoteType
    {
        Outcoming = 1,
        Incoming = 2
    }
}