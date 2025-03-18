using MeterReadingApp.Models;

namespace MeterReadingApp.Repositories
{
    public interface IAccountRepository
    {
        Task<List<Account>> RetrieveAccounts(List<int> accountIds);
    }
}
