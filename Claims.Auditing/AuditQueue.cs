using Claims.Auditing.Abstractions;
using System.Threading.Channels;

namespace Claims.Auditing
{
    /// <inheritdoc cref="IAuditQueue"/>
    public class AuditQueue : IAuditQueue
    {
        // Bounded so a burst of writes can't grow memory unbounded.
        // DropOldest keeps the app responsive under load — losing an old audit
        // entry is preferable to blocking or crashing the API.
        private readonly Channel<AuditEntry> _channel = Channel.CreateBounded<AuditEntry>(
            new BoundedChannelOptions(capacity: 1000)
            {
                SingleReader = true,
                SingleWriter = false,
                FullMode = BoundedChannelFullMode.DropOldest
            });

        public void Enqueue(AuditEntry entry) => _channel.Writer.TryWrite(entry);

        public IAsyncEnumerable<AuditEntry> DequeueAllAsync(CancellationToken cancellationToken) 
            => _channel.Reader.ReadAllAsync(cancellationToken);
    }
}
