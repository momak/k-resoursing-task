namespace Claims.Data.Abstractions
{
    /// <summary>
    /// Provides data access for <see cref="Claim"/> entities.
    /// </summary>
    public interface IClaimsRepository
    {
        /// <summary>
        /// Asynchronously retrieves all <see cref="Claim"/> entities from the data store.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// a collection of all <see cref="Claim"/> entities.
        /// </returns>
        Task<IEnumerable<Claim>> GetAllAsync();

        /// <summary>
        /// Asynchronously retrieves the <see cref="Claim"/> with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the <see cref="Claim"/> to locate.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// the matching <see cref="Claim"/>, or <see langword="null"/> if no claim
        /// with the given <paramref name="id"/> exists.
        /// </returns>
        Task<Claim?> GetByIdAsync(string id);

        /// <summary>
        /// Asynchronously adds a new <see cref="Claim"/> to the data store.
        /// </summary>
        /// <param name="claim">The <see cref="Claim"/> to add. Cannot be <see langword="null"/>.</param>
        /// <returns>
        /// A task that represents the asynchronous add operation.
        /// </returns>
        /// <remarks>
        /// On a successful insert, the data store may populate <see cref="Claim.Id"/>
        /// on the supplied instance.
        /// </remarks>
        Task AddAsync(Claim claim);

        /// <summary>
        /// Asynchronously removes the specified <see cref="Claim"/> from the data store.
        /// </summary>
        /// <param name="claim">The <see cref="Claim"/> to delete. Cannot be <see langword="null"/>.</param>
        /// <returns>
        /// A task that represents the asynchronous delete operation.
        /// </returns>
        /// <remarks>
        /// If <paramref name="claim"/> does not exist in the data store, the operation
        /// is a no-op.
        /// </remarks>
        Task DeleteAsync(Claim claim);
    }
}
