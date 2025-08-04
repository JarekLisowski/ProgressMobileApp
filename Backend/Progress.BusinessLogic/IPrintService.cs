
namespace Progress.BusinessLogic
{
  public interface IPrintService
  {
    Guid GenerateInvoicePrintout(int dokId);

    Guid GenerateCashReceiptPrintout(int nzId);
    Printout GetPrintout(string guid);
  }
}