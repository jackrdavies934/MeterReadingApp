using System.Globalization;
using MeterReadingApp.Repositories;
using CsvHelper;
using CsvHelper.Configuration;
using MeterReadingApp.Models;
using MeterReadingApp.Models.DTO;
using AutoMapper;

namespace MeterReadingApp.Services
{
    public class MeterReadingService : IMeterReadingService
    {
        private readonly IMeterReadingRepository _meterReadingRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MeterReadingService> _logger;

        public MeterReadingService(IMeterReadingRepository meterReadingRepository, IAccountRepository accountRepository, IMapper mapper, ILogger<MeterReadingService> logger)
        {
            _meterReadingRepository = meterReadingRepository;
            _accountRepository = accountRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<MeterReadingResult> ProcessMeterReadings(IFormFile file)
        {
            var meterReadingsDTO = new List<MeterReadingDTO>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);

                using (var reader = new StreamReader(stream))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.CurrentCulture)))
                {
                    meterReadingsDTO = csv.GetRecords<MeterReadingDTO>().ToList();
                }
            }

            //Automapper to map from the DTO to the Data Entity
            var meterReadings = _mapper.Map<List<MeterReading>>(meterReadingsDTO);

            var validMeterReadings = await ValidateMeterReadings(meterReadings);

            var successCount = 0;
            if (validMeterReadings.Count > 0)
            {
                successCount = await _meterReadingRepository.SaveMeterReadings(validMeterReadings);
            }

            var failureCount = meterReadings.Count - successCount;

            return new MeterReadingResult
            {
                Success = successCount > 0,
                SuccessCount = successCount,
                FailureCount = failureCount
            };
        }

        public async Task<List<MeterReading>> ValidateMeterReadings(List<MeterReading> meterReadings)
        {
            try
            {
                var accountIds = meterReadings.Select(m => m.AccountId).ToList();
                var existingAccounts = await _accountRepository.RetrieveAccounts(accountIds);
                var existingMeterReadings = await _meterReadingRepository.RetrieveMeterReadingsByAccountIds(accountIds);

                var validMeterReadings = meterReadings
                    .Where(m => IsValidMeterReading(m, existingAccounts, existingMeterReadings))
                    .ToList();

                return validMeterReadings;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred: {ex.Message}");
                throw new Exception("An unexpected error occurred.");
            }
        }

        private bool IsValidMeterReading(MeterReading meterReading, List<Account> existingAccounts, List<MeterReading> existingMeterReadings)
        {
            //Check for Duplicates - "You should not be able to load the same data entry twice" (See Assumptions)
            var duplicate = existingMeterReadings
                .Any(e => e.MeterReadingDateTime == meterReading.MeterReadingDateTime
                       && e.MeterReadValue == meterReading.MeterReadValue
                       && e.MeterReadingDateTime == meterReading.MeterReadingDateTime);

            if (duplicate)
            {
                return false;
            }

            //Check Account Exists - "A meter reading must be associated with an Account ID to be deemed valid"
            if (!existingAccounts.Any(a => a.AccountId == meterReading.AccountId))
            {
                return false;
            }

            // Check if MeterReadValue is valid (length <= 5) - "Reading values should be in the format NNNNN (See Assumptions)
            if (meterReading.MeterReadValue.Length > 5)
            {
                return false;
            }

            //Nice to have - "When an account has an existing read, ensure the new read isn't older than the existing read
            var existingAccountMeterReadings = existingMeterReadings
                .Where(e => e.AccountId == meterReading.AccountId)
                .ToList();

            if (existingAccountMeterReadings.Any(e => e.MeterReadingDateTime >= meterReading.MeterReadingDateTime))
            {
                return false;
            }

            return true;
        }
    }
}
