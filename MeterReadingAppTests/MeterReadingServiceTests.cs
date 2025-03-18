using AutoMapper;
using MeterReadingApp.Models;
using MeterReadingApp.Repositories;
using MeterReadingApp.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace MeterReadingAppTests
{
    [TestFixture]
    public class MeterReadingServiceTests
    {
        private Mock<IAccountRepository> _mockAccountRepo;
        private Mock<IMeterReadingRepository> _mockMeterReadingRepo;
        private Mock<IMapper> _mockMapper;
        private Mock<ILogger<MeterReadingService>> _mockLogger;
        private MeterReadingService _service;

        [SetUp]
        public void SetUp()
        {
            _mockAccountRepo = new Mock<IAccountRepository>();
            _mockMeterReadingRepo = new Mock<IMeterReadingRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<MeterReadingService>>();

            _service = new MeterReadingService(_mockMeterReadingRepo.Object, _mockAccountRepo.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Test]
        public async Task ValidateMeterReadings_ValidInput_ReturnsValidMeterReadings()
        {
            // Arrange
            var meterReadings = new List<MeterReading>
            {
                new MeterReading { AccountId = 1, MeterReadingDateTime = DateTime.Now, MeterReadValue = "1234" },
                new MeterReading { AccountId = 2, MeterReadingDateTime = DateTime.Now, MeterReadValue = "12345" }
                };

            var existingAccounts = new List<Account>
                {
                new Account { AccountId = 1 },
                new Account { AccountId = 2 }
            };

            _mockAccountRepo.Setup(x => x.RetrieveAccounts(It.IsAny<List<int>>())).ReturnsAsync(existingAccounts);
            _mockMeterReadingRepo.Setup(x => x.RetrieveMeterReadingsByAccountIds(It.IsAny<List<int>>())).ReturnsAsync([]);

            // Act
            var result = await _service.ValidateMeterReadings(meterReadings);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// This unit test will test the Acceptance Criteria for "A meter reading must be associated with an Account ID to be deemed valid"
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task ValidateMeterReadings_InvalidAccountId_ReturnsEmptyList()
        {
            // Arrange
            var meterReadings = new List<MeterReading>
            {
                new MeterReading { AccountId = 3, MeterReadingDateTime = DateTime.Now, MeterReadValue = "1234" } //Invalid Accountid
            };

            var existingAccounts = new List<Account>
                {
                new Account { AccountId = 1 },
                new Account { AccountId = 2 }
            };

            _mockAccountRepo.Setup(x => x.RetrieveAccounts(It.IsAny<List<int>>())).ReturnsAsync(existingAccounts);

            // Act
            var result = await _service.ValidateMeterReadings(meterReadings);

            // Assert
            Assert.That(result, Is.Empty);
        }

        /// <summary>
        /// This unit test will test the Acceptance Criteria for "Reading values should be in the format NNNNN"
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task ValidateMeterReadings_InvalidMeterReadValue_ReturnsEmptyList()
        {
            // Arrange
            var meterReadings = new List<MeterReading>
            {
                new MeterReading { AccountId = 1, MeterReadingDateTime = DateTime.Now, MeterReadValue = "123456" } //Invalid meter read value
            };

            var existingAccounts = new List<Account>
            {
                new Account { AccountId = 1 }
            };

            var existingMeterReadings = new List<MeterReading>();

            _mockAccountRepo.Setup(x => x.RetrieveAccounts(It.IsAny<List<int>>())).ReturnsAsync(existingAccounts);
            _mockMeterReadingRepo.Setup(x => x.RetrieveMeterReadingsByAccountIds(It.IsAny<List<int>>())).ReturnsAsync(existingMeterReadings);

            // Act
            var result = await _service.ValidateMeterReadings(meterReadings);

            // Assert
            Assert.That(result, Is.Empty);
        }

        /// <summary>
        /// This unit test will test the Acceptance Criteria validation for "You should not be able to load the same entry twice
        /// </summary>
        /// <returns>Empty MeterReading List</returns>
        [Test]
        public async Task ValidateMeterReadings_DuplicateMeterReading_ReturnsEmptyList()
        {
            // Arrange
            var meterReadings = new List<MeterReading>
            {
                new MeterReading { AccountId = 1, MeterReadingDateTime = DateTime.Now, MeterReadValue = "1234" } //Duplicate meter reading
            };

            var existingAccounts = new List<Account>
            {
                new Account { AccountId = 1 }
            };

            var existingMeterReadings = new List<MeterReading>
            {
                new MeterReading { AccountId = 1, MeterReadingDateTime = DateTime.Now, MeterReadValue = "1234" }
            };

            _mockAccountRepo.Setup(x => x.RetrieveAccounts(It.IsAny<List<int>>())).ReturnsAsync(existingAccounts);
            _mockMeterReadingRepo.Setup(x => x.RetrieveMeterReadingsByAccountIds(It.IsAny<List<int>>())).ReturnsAsync(existingMeterReadings);

            // Act
            var result = await _service.ValidateMeterReadings(meterReadings);

            // Assert
            Assert.That(result.Count, Is.EqualTo(0));
        }
    }
}