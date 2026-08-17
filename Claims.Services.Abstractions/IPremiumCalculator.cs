namespace Claims.Services.Abstractions
{
    /// <summary>
    /// Calculates insurance premiums based on cover parameters.
    /// </summary>
    public interface IPremiumCalculator
    {
        /// <summary>
        /// Calculates the insurance premium based on the provided start date, end date, and cover type.
        /// </summary>
        /// <param name="startDate">The start date of the coverage period.</param>
        /// <param name="endDate">The end date of the coverage period.</param>
        /// <param name="coverType">The type of cover.</param>
        /// <returns>The calculated insurance premium.</returns>
        decimal Compute(DateTime startDate, DateTime endDate, CoverType coverType);
    }
}
