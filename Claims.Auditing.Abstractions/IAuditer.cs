namespace Claims.Auditing.Abstractions
{
    /// <summary>
    /// Interface for auditing claims and covers.
    /// </summary>
    public interface IAuditer
    {
        /// <summary>
        /// Audits a claim with the specified ID and action.
        /// </summary>
        /// <param name="id">The ID of the claim to audit.</param>
        /// <param name="action">The action performed on the claim.</param>
        void AuditClaim(string id, string action);

        /// <summary>
        /// Audits a cover with the specified ID and action.
        /// </summary>
        /// <param name="id">The ID of the cover to audit.</param>
        /// <param name="action">The action performed on the cover.</param>
        void AuditCover(string id, string action);
    }
}
