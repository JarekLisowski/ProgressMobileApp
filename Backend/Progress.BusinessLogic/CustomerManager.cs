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

    public CustomerManager(IDatabaseRepository<Customer, IfVwKontrahent> repoKontrahent, CustomerRepository repoCustomer)
    {
      dbKontrahent = repoKontrahent;
      customerRepository = repoCustomer;
    }

    public Customer? Get(int id)
    {
      var data = dbKontrahent.Select(id);
      return data;
    }

    public IEnumerable<Customer> Search(string pattern, int? cechaId = null, int? limit = null)
    {
      Customer[] data = customerRepository.GetCustomers(pattern, cechaId, limit);
      return data;
    }
  }
}
