using Progress.Domain.Navireo;

namespace Progress.Navireo.Extensions
{
    public static class Dokument
    {
        public static DocumentEnum GetDocumentEnumType(this InsERT.SuDokument dok)
        {
            return dok.Typ.GetDocumentEnumType();
        }

        //public static DocumentEnum DocumentEnumType(this Model.IF_vwDokument dok)
        //{
        //    return GetDocumentEnumType(dok.dok_Typ);
        //}
        
        public static DocumentEnum GetDocumentEnumType(this int typ)
        {
            if (typ == 16) return DocumentEnum.CustomerOrder;
            else if (typ == 2) return DocumentEnum.SalesInvoice;
            else if (typ == 1) return DocumentEnum.PurchaseInvoice;
            else if (typ == 6) return DocumentEnum.SalesInvoiceCorrection;
            else if (typ == 5) return DocumentEnum.PurchaseInvoiceCorrection;
            else if (typ == 21) return DocumentEnum.Receipt;
            else if (typ == 51) return DocumentEnum.CustomerInquiry;
            else if (typ == 15) return DocumentEnum.StoreOrder;
            throw new Exception(string.Format("Not implemented type of document (typ: {0})", typ));
        }

        public static int TypDokumentu(this DocumentEnum dokEum)
        {
            if (dokEum == null) return 0;
            return ((DocumentEnum?)dokEum).TypDokumentu();
        }

        public static int TypDokumentu(this DocumentEnum? dokEum)
        {
            if (dokEum == null) return 0;
            if (dokEum == DocumentEnum.CustomerOrder) return 16;
            else if (dokEum == DocumentEnum.PurchaseInvoice) return 1;
            else if (dokEum == DocumentEnum.SalesInvoice) return 2;
            else if (dokEum == DocumentEnum.SalesInvoiceCorrection) return 6;
            else if (dokEum == DocumentEnum.PurchaseInvoiceCorrection) return 5;
            else if (dokEum == DocumentEnum.Receipt) return 21;
            else if (dokEum == DocumentEnum.CustomerOffer) return 51;
            else if (dokEum == DocumentEnum.StoreOrder) return 15;
            throw new Exception(string.Format("Not implemented type of document : '{0}'", dokEum.ToString()));
        }


        public static int PodtypDokumentu(this DocumentEnum dokEum)
        {
            if (dokEum == null) return 0;
            return ((DocumentEnum?)dokEum).PodtypDokumentu();
        }

        public static int PodtypDokumentu(this DocumentEnum? dokEum)
        {
            if (dokEum == DocumentEnum.CustomerOrder) return 0;
            else if (dokEum == DocumentEnum.PurchaseInvoice) return 0;
            else if (dokEum == DocumentEnum.SalesInvoice) return 0;
            else if (dokEum == DocumentEnum.SalesInvoiceCorrection) return 0;
            else if (dokEum == DocumentEnum.PurchaseInvoiceCorrection) return 0;
            else if (dokEum == DocumentEnum.Receipt) return 2;
            else if (dokEum == DocumentEnum.CustomerOffer) return 0;
            else if (dokEum == DocumentEnum.StoreOrder) return 0;
            throw new Exception(string.Format("Not implemented type of document : '{0}'", dokEum.ToString()));
        }
    }
}
