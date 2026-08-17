using Claims.Auditing.Abstractions;
using Moq;

namespace Claims.Auditing.Tests
{
    public class AuditerTests
    {
        private readonly Mock<IAuditQueue> _queue = new();
        private readonly Auditer _sut;

        public AuditerTests()
        {
            _sut = new Auditer(_queue.Object);
        }

        [Fact]
        public void AuditClaim_EnqueuesClaimAuditEntry()
        {
            // Arrange
            const string claimId = "claim-1";
            const string action = "POST";

            // Act
            _sut.AuditClaim(claimId, action);

            // Assert
            _queue.Verify(q => q.Enqueue(It.Is<ClaimAuditEntry>(e =>
                e.EntityId == claimId && e.Action == action)), Times.Once);
        }

        [Fact]
        public void AuditCover_EnqueuesCoverAuditEntry()
        {
            // Arrange
            const string coverId = "cover-1";
            const string action = "DELETE";

            // Act
            _sut.AuditCover(coverId, action);

            // Assert
            _queue.Verify(q => q.Enqueue(It.Is<CoverAuditEntry>(e =>
                e.EntityId == coverId && e.Action == action)), Times.Once);
        }
    }
}
