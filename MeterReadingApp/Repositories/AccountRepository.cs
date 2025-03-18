using MeterReadingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MeterReadingApp.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;

        public AccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Account>> RetrieveAccounts(List<int> accountIds)
        {
            return await _context.Accounts
                .Where(a => accountIds.Contains(a.AccountId))
                .ToListAsync();
        }
    }
}
