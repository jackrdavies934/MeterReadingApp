using MeterReadingApp.Models;

namespace MeterReadingApp.Services
{
    public interface IMeterReadingService
    {
        Task<MeterReadingResult> ProcessMeterReadings(IFormFile file);
    }
}
