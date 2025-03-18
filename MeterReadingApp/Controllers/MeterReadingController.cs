using MeterReadingApp.Services;
using MeterReadingApp.Validation;
using Microsoft.AspNetCore.Mvc;

namespace MeterReadingApp.Controllers;

[ApiController]
[Route("[controller]")]
public class MeterReadingController : ControllerBase
{
    private readonly IMeterReadingService _meterReadingService;
    private readonly IMeterReadingValidator _meterReadingValidator;
    private readonly ILogger<MeterReadingController> _logger;

    public MeterReadingController(IMeterReadingService meterReadingService, IMeterReadingValidator meterReadingValidator, ILogger<MeterReadingController> logger)
    {
        _meterReadingService = meterReadingService;
        _meterReadingValidator = meterReadingValidator;
        _logger = logger;
    }

    [HttpPost("meter-reading-uploads")]
    public async Task<IActionResult> UploadMeterReading(IFormFile file)
    {
        try
        {
            //Fluent Validation to check for invalid requests
            var validationResult = _meterReadingValidator.Validate(file);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            //Call out to business logic
            var result = await _meterReadingService.ProcessMeterReadings(file);

            if (result.Success)
            {
                return Ok(new { message = "File uploaded and data saved.", SuccessfulRecordCount = result.SuccessCount, FailureRecordCount = result.FailureCount });
            }
            return Ok(new { message = "No records saved", SuccessfulRecordCount = result.SuccessCount, FailureRecordCount = result.FailureCount });
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while saving meter readings: {ex.Message}");
            return StatusCode(500, "Internal server error while processing your request.");
        }
    }
}
