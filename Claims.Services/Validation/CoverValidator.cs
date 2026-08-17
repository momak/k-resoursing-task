using FluentValidation;

namespace Claims.Services.Validation
{
    /// <summary>
    /// Validates business rules for a <see cref="Cover"/> before it is persisted.
    /// </summary>
    public class CoverValidator : AbstractValidator<Cover>
    {
        private const int MaxPeriodDays = 365;

        public CoverValidator()
        {
            RuleFor(cover => cover.StartDate)
                .GreaterThanOrEqualTo(_ => DateTime.UtcNow.Date)
                .WithMessage("Cover start date cannot be in the past.");

            RuleFor(cover => cover)
                .Must(cover => (cover.EndDate - cover.StartDate).TotalDays <= MaxPeriodDays)
                .WithMessage($"Cover period cannot exceed {MaxPeriodDays} days.");
        }
    }
}
