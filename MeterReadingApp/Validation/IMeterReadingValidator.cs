using FluentValidation.Results;

namespace MeterReadingApp.Validation
{
    public interface IMeterReadingValidator
    {
        ValidationResult Validate(IFormFile file);
    }
}
