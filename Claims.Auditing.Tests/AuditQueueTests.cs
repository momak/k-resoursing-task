using Claims.Auditing.Abstractions;

namespace Claims.Auditing.Tests
{
    public class AuditQueueTests
    {
        private readonly AuditQueue _sut = new();

        [Fact]
        public async Task Enqueue_ThenDequeueAllAsync_ReturnsTheEnqueuedEntry()
        {
            // Arrange
            var entry = new ClaimAuditEntry("claim-1", "POST", DateTime.UtcNow);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

            // Act
            _sut.Enqueue(entry);

            var received = new List<AuditEntry>();
            await foreach (var item in _sut.DequeueAllAsync(cts.Token))
            {
                received.Add(item);
                break; // stop after the first item since the channel never completes on its own
            }

            // Assert
            Assert.Single(received);
            Assert.Equal(entry, received[0]);
        }

        [Fact]
        public async Task Enqueue_MultipleEntries_AreReceivedInOrder()
        {
            // Arrange
            var first = new ClaimAuditEntry("claim-1", "POST", DateTime.UtcNow);
            var second = new CoverAuditEntry("cover-1", "DELETE", DateTime.UtcNow);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

            // Act
            _sut.Enqueue(first);
            _sut.Enqueue(second);

            var received = new List<AuditEntry>();
            await foreach (var item in _sut.DequeueAllAsync(cts.Token))
            {
                received.Add(item);
                if (received.Count == 2) break;
            }

            // Assert
            Assert.Equal([first, second], received);
        }
    }
}
