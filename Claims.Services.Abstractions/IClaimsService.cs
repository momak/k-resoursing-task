namespace Claims.Services.Abstractions
{
    /// <summary>
    /// Defines the contract for a service that manages claims.
    /// </summary>
    public interface IClaimsService
    {
        /// <summary>
        /// Retrieves all claims asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of claims.</returns>
        Task<IEnumerable<Claim>> GetClaimsAsync();

        /// <summary>
        /// Retrieves a specific claim by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the claim to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the claim if found; otherwise, null.</returns>
        Task<Claim?> GetClaimAsync(string id);

        /// <summary>
        /// Creates a new claim asynchronously.
        /// </summary>
        /// <param name="claim">The claim to create.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created claim.</returns>
        Task<Claim> CreateClaimAsync(Claim claim);

        /// <summary>
        /// Deletes a claim by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the claim to retrieve.</param>
        Task DeleteClaimAsync(string id);
    }
}
