
namespace Progress.BusinessLogic
{
  public interface IPrintService
  {
    Guid GenerateInvoicePrintout(int dokId);
    Printout GetPrintout(string guid);
  }
}