using Progress.Domain.Model;
using Progress.Infrastructure.Database.Repository;

namespace Progress.BusinessLogic
{
	public class FinanceManager
	{
    private FinanceRepository _financeRepository;
    private CustomerManager _customerManager;
    private UserRepository _userRepository;

    public FinanceManager(FinanceRepository financeRepository, CustomerManager customerManager, UserRepository userRepository)
    {
      _customerManager = customerManager;
      _financeRepository = financeRepository;
      _userRepository = userRepository;
    }

    public CashReceipt? GetCashReceipt(int id)
    {
      var receipt = _financeRepository.GetCashReceipt(id);
      if (receipt != null)
      {
        receipt.Seller = _customerManager.GetOwnCompany();
        return receipt;
      }
      return null;
    }
  }
}
