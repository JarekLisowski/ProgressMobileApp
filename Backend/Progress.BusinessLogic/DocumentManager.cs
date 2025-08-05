using Progress.Domain.Extensions;
using Progress.Domain.Model;
using Progress.Infrastructure.Database.Repository;

namespace Progress.BusinessLogic
{
	public class DocumentManager
	{
    DocumentRepository _documentRepository;
    CustomerRepository _customerRepository;
    UserRepository _userRepository;
    public DocumentManager(DocumentRepository documentRepository, CustomerRepository customerRepository, UserRepository userRepository)
    {
      _documentRepository = documentRepository;
      _customerRepository = customerRepository;
      _userRepository = userRepository;
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

    public Document[] GetInternalOrders(int userId)
    {
      return _documentRepository.GetDocuments(15, 1, userId);
    }
  }
}
