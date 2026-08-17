namespace Claims.Services.Abstractions
{
    /// <summary>
    /// Orchestrates cover retrieval, creation, deletion, and premium calculation.
    /// </summary>
    public interface ICoversService
    {
        /// <summary>
        /// Retrieves all covers asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of covers.</returns>
        Task<IEnumerable<Cover>> GetCoversAsync();

        /// <summary>
        /// Retrieves a specific cover by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the cover to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the cover with the specified ID, or null if not found.</returns>
        Task<Cover?> GetCoverAsync(string id);

        /// <summary>
        /// Creates a new cover asynchronously.
        /// </summary>
        /// <param name="cover">The cover to create.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created cover.</returns>
        Task<Cover> CreateCoverAsync(Cover cover);

        /// <summary>
        /// Deletes a cover by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the cover to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeleteCoverAsync(string id);

        /// <summary>
        /// Computes the premium for a cover based on the provided parameters.
        /// </summary>
        /// <param name="startDate">The start date of the coverage period.</param>
        /// <param name="endDate">The end date of the coverage period.</param>
        /// <param name="coverType">The type of cover.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the computed premium amount.</returns>
        Task<decimal> ComputePremiumAsync(DateTime startDate, DateTime endDate, CoverType coverType);
    }
}
