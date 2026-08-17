using Claims.Services.Abstractions;

namespace Claims.Services.Services
{
    /// <inheritdoc cref="IPremiumCalculator"/>
    public class PremiumCalculator : IPremiumCalculator
    {
        private const decimal BaseDayRate = 1250m;
        private const int FullRateDays = 30;
        private const int DiscountedPeriodDays = 150;


        /// <inheritdoc cref="IPremiumCalculator.Compute"/>
        public decimal Compute(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            var dayRate = BaseDayRate * GetTypeMultiplier(coverType);
            var totalDays = (int)(endDate - startDate).TotalDays;

            var fullRateDays = Math.Min(totalDays, FullRateDays);
            var discountedDays = Math.Clamp(totalDays - FullRateDays, 0, DiscountedPeriodDays);
            var deeplyDiscountedDays = Math.Max(totalDays - FullRateDays - DiscountedPeriodDays, 0);

            var (discountedRate, additionalDiscount) = GetDiscountRates(coverType);
            var deeplyDiscountedRate = discountedRate + additionalDiscount;

            return fullRateDays * dayRate
                 + discountedDays * dayRate * (1 - discountedRate)
                 + deeplyDiscountedDays * dayRate * (1 - deeplyDiscountedRate);
        }

        private static decimal GetTypeMultiplier(CoverType coverType) => coverType switch
        {
            CoverType.Yacht => 1.1m,
            CoverType.PassengerShip => 1.2m,
            CoverType.Tanker => 1.5m,
            _ => 1.3m
        };

        /// <summary>
        /// Returns the discount for days 31–180, and the additional discount stacked on top
        /// for day 181 onward.
        /// </summary>
        private static (decimal DiscountedRate, decimal AdditionalDiscount) GetDiscountRates(CoverType coverType)
            => coverType == CoverType.Yacht
                ? (0.05m, 0.03m)
                : (0.02m, 0.01m);
    }
}
