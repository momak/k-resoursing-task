namespace Claims.Auditing.Abstractions
{
    /// <summary>
    /// In-memory producer/consumer queue decoupling audit-writing from the HTTP request.
    /// </summary>
    public interface IAuditQueue
    {
        /// <summary>
        /// Enqueues an entry. Non-blocking — returns immediately.
        /// </summary>
        void Enqueue(AuditEntry entry);

        /// <summary>
        /// Streams entries as they arrive. Used by the background consumer only.
        /// </summary>
        IAsyncEnumerable<AuditEntry> DequeueAllAsync(CancellationToken cancellationToken);
    }
}
