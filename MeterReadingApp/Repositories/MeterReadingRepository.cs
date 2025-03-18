using MeterReadingApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace MeterReadingApp.Repositories
{
    public class MeterReadingRepository : IMeterReadingRepository
    {
        private readonly ApplicationDbContext _context;

        public MeterReadingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MeterReading>> RetrieveMeterReadingsByAccountIds(List<int> accountIds)
        {
            return await _context.MeterReadings
                .Where(m => accountIds.Contains(m.AccountId))
                .ToListAsync();
        }

        public async Task<int> SaveMeterReadings(List<MeterReading> meterReadings)
        {
            await _context.MeterReadings.AddRangeAsync(meterReadings);
            return await _context.SaveChangesAsync();
        }
    }
}
