using Claims.Auditing.Abstractions;

namespace Claims.Auditing
{
    /// <inheritdoc cref="IAuditer"/>
    public class Auditer : IAuditer
    {
        private readonly IAuditQueue _queue;

        /// <summary>
        /// Initializes a new instance of the <see cref="Auditer"/> class.
        /// </summary>
        /// <param name="_queue">The audit queue.</param>
        public Auditer(IAuditQueue queue)    
        {
            _queue = queue;
        }

        /// <inheritdoc cref="IAuditer.AuditClaim"/>
        public void AuditClaim(string id, string action)
            => _queue.Enqueue(new ClaimAuditEntry(id, action,DateTime.UtcNow));

        /// <inheritdoc cref="IAuditer.AuditCover(string, string)"/>
        public void AuditCover(string id, string action)
        => _queue.Enqueue(new CoverAuditEntry(id, action, DateTime.UtcNow));
    }
}
