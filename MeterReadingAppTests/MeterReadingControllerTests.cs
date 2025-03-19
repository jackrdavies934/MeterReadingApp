using MeterReadingApp.Controllers;
using MeterReadingApp.Services;
using MeterReadingApp.Validation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using MeterReadingApp.Models;

namespace MeterReadingApp.Tests
{
    [TestFixture]
    public class MeterReadingControllerTests
    {
        private Mock<IMeterReadingService> _mockMeterReadingService;
        private Mock<IMeterReadingValidator> _mockMeterReadingValidator;
        private Mock<ILogger<MeterReadingController>> _mockLogger;
        private MeterReadingController _controller;

        [SetUp]
        public void Setup()
        {
            _mockMeterReadingService = new Mock<IMeterReadingService>();
            _mockMeterReadingValidator = new Mock<IMeterReadingValidator>();
            _mockLogger = new Mock<ILogger<MeterReadingController>>();

            _controller = new MeterReadingController(_mockMeterReadingService.Object, _mockMeterReadingValidator.Object, _mockLogger.Object);
        }

        [Test]
        public async Task UploadMeterReading_ValidFile_ReturnsOkResult()
        {
            // Arrange
            var file = new Mock<IFormFile>();
            var processResult = new MeterReadingResult { Success = true, SuccessCount = 5, FailureCount = 0 };

            _mockMeterReadingValidator.Setup(v => v.Validate(It.IsAny<IFormFile>())).Returns(new FluentValidation.Results.ValidationResult());
            _mockMeterReadingService.Setup(s => s.ProcessMeterReadings(It.IsAny<IFormFile>())).ReturnsAsync(processResult);

            // Act
            var result = await _controller.UploadMeterReading(file.Object);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            var response = okResult.Value;
        }

        [Test]
        public async Task UploadMeterReading_InvalidFile_ReturnsBadRequest()
        {
            // Arrange
            var file = new Mock<IFormFile>();
            var validationResult = new FluentValidation.Results.ValidationResult();
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("File", "Invalid file"));

            _mockMeterReadingValidator.Setup(v => v.Validate(It.IsAny<IFormFile>())).Returns(validationResult);

            // Act
            var result = await _controller.UploadMeterReading(file.Object);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.That(badRequestResult.Value, Is.EqualTo(validationResult.Errors));
        }
    }
}