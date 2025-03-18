using FluentValidation;

namespace MeterReadingApp.Validation
{
    public class MeterReadingValidator : AbstractValidator<IFormFile>, IMeterReadingValidator
    {
        public MeterReadingValidator()
        {
            RuleFor(x => x)
                .NotNull().WithMessage("No file uploaded.")
                .Must(f => f.Length > 0).WithMessage("File cannot be empty.")
                .Must(f => f.ContentType == "text/csv").WithMessage("Please upload a CSV file.");
        }
    }
}
