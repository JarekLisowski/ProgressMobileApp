using Progress.Domain.Extensions;
using Progress.Domain.Model;
using Progress.Infrastructure.Database.Repository;

namespace Progress.BusinessLogic
{
	public class DocumentManager
	{
    DocumentRepository _documentRepository;
    CustomerRepository _customerRepository;
    public DocumentManager(DocumentRepository documentRepository, CustomerRepository customerRepository)
    {
      _documentRepository = documentRepository;
      _customerRepository = customerRepository;
    }

    public Document? GetDocument(int id)
    {
      var document = _documentRepository.GetDocument(id);
      if (document != null)
      {
        var ownCompany = _customerRepository.GetOwnCompany();
        document.Seller = ownCompany;
        document.CalculateVatSummary();
        return document;
      }
      return null;
    }
  }
}
