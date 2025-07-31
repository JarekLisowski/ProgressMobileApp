using Progress.Database;
using Progress.Domain.Model;
using Progress.Infrastructure.Database.Repository;

namespace Progress.BusinessLogic
{
	public class CustomerManager
	{
    IDatabaseRepository<Customer, IfVwKontrahent> dbKontrahent;
    IDatabaseRepository<Addres, AdrEwid> dbAdres;
    CustomerRepository customerRepository;
    static Customer? _ownCompany = null;

    public CustomerManager(IDatabaseRepository<Customer, IfVwKontrahent> repoKontrahent, CustomerRepository repoCustomer)
    {
      dbKontrahent = repoKontrahent;
      customerRepository = repoCustomer;
    }

    public Customer? Get(int id)
    {
      var data = dbKontrahent.SelectWhere(it => it.KhId == id, true).FirstOrDefault();
      return data;
    }

    public IEnumerable<Customer> Search(string pattern, int? cechaId = null, int? limit = null)
    {
      Customer[] data = customerRepository.GetCustomers(pattern, cechaId, limit);
      return data;
    }

    public Customer GetOwnCompany()
    {
      if (_ownCompany == null)
      {
        _ownCompany = customerRepository.GetOwnCompany();
      }
      return _ownCompany;
    }

  }
}
