using Claims.Data.Abstractions;
using FluentValidation;

namespace Claims.Services.Validation
{
    /// <summary>
    /// Validates business rules for a <see cref="Claim"/>, including checks against its related <see cref="Cover"/>.
    /// </summary>
    public class ClaimValidator : AbstractValidator<Claim>
    {
        private const decimal MaxDamageCost = 100_000m;

        public ClaimValidator(ICoversRepository coversRepository)
        {
            RuleFor(claim => claim.DamageCost)
                .LessThanOrEqualTo(MaxDamageCost)
                .WithMessage($"Damage cost cannot exceed {MaxDamageCost:N0}.");

            RuleFor(claim => claim)
                .MustAsync(async (claim, cancellation) =>
                {
                    var cover = await coversRepository.GetByIdAsync(claim.CoverId);
                    return cover is not null
                        && claim.Created.Date >= cover.StartDate.Date
                        && claim.Created.Date <= cover.EndDate.Date;
                })
                .WithMessage("Claim created date must fall within the related cover's period.");
        }
    }
}
