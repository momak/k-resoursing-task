namespace Claims.Data.Abstractions
{
    /// <summary>
    /// Provides data access for <see cref="Cover"/> entities.
    /// </summary>
    public interface ICoversRepository
    {
        /// <summary>
        /// Asynchronously retrieves all <see cref="Cover"/> entities from the data store.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// a collection of all <see cref="Cover"/> entities.
        /// </returns>
        Task<IEnumerable<Cover>> GetAllAsync();

        /// <summary>
        /// Asynchronously retrieves the <see cref="Cover"/> with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the <see cref="Cover"/> to locate.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// the matching <see cref="Cover"/>, or <see langword="null"/> if no cover
        /// with the given <paramref name="id"/> exists.
        /// </returns>
        Task<Cover?> GetByIdAsync(string id);

        /// <summary>
        /// Asynchronously adds a new <see cref="Cover"/> to the data store.
        /// </summary>
        /// <param name="cover">The <see cref="Cover"/> to add. Cannot be <see langword="null"/>.</param>
        /// <returns>
        /// A task that represents the asynchronous add operation.
        /// </returns>
        Task AddAsync(Cover cover);

        /// <summary>
        /// Asynchronously removes the specified <see cref="Cover"/> from the data store.
        /// </summary>
        /// <param name="cover">The <see cref="Cover"/> to delete. Cannot be <see langword="null"/>.</param>
        /// <returns>
        /// A task that represents the asynchronous delete operation.
        /// </returns>
        /// <remarks>
        /// If <paramref name="cover"/> does not exist in the data store, the operation
        /// is a no-op.
        /// </remarks>
        Task DeleteAsync(Cover cover);
    }
}