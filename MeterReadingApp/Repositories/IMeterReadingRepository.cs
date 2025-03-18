using MeterReadingApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MeterReadingApp.Repositories
{
    public interface IMeterReadingRepository
    {
        Task<List<MeterReading>> RetrieveMeterReadingsByAccountIds(List<int> accountIds);
        Task<int> SaveMeterReadings(List<MeterReading> meterReadings);
    }
}
