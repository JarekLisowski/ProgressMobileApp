using Progress.Domain.Navireo;
using Progress.Navireo.Extensions;

namespace Progress.Navireo.Constants
{
    public class Document
    {
        /// <summary>
        /// Obsługiwane typy dokumentów
        /// </summary>
        public static IEnumerable<DocumentEnum> DokumentEnumList = new List<DocumentEnum>()
        {
            DocumentEnum.CustomerOrder, 
            DocumentEnum.SalesInvoice,
            DocumentEnum.Receipt,
            DocumentEnum.StoreOrder
        };

        public static IEnumerable<int> DokumentTypList
        {
            get
            {
                return DokumentEnumList.Select(it => it.TypDokumentu());
            }
        }
    }
}
