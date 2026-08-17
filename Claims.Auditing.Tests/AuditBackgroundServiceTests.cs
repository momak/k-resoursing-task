using Claims.Auditing.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Claims.Auditing.Tests
{
    public class AuditBackgroundServiceTests : IClassFixture<SqlServerFixture>, IAsyncLifetime
    {
        private readonly SqlServerFixture _fixture;
        private ServiceProvider _provider = null!;
        private AuditQueue _queue = null!;
        private AuditBackgroundService _sut = null!;
        private CancellationTokenSource _cts = null!;
        private Task _executeTask = null!;

        public AuditBackgroundServiceTests(SqlServerFixture fixture)
        {
            _fixture = fixture;
        }

        public Task InitializeAsync()
        {
            var services = new ServiceCollection();
            services.AddScoped(_ => _fixture.CreateContext());
            _provider = services.BuildServiceProvider();

            _queue = new AuditQueue();
            _sut = new AuditBackgroundService(
                _queue,
                _provider.GetRequiredService<IServiceScopeFactory>(),
                NullLogger<AuditBackgroundService>.Instance);

            _cts = new CancellationTokenSource();
            _executeTask = _sut.StartAsync(_cts.Token);

            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            _cts.Cancel();
            await _sut.StopAsync(CancellationToken.None);
            await _provider.DisposeAsync();
        }

        [Fact]
        public async Task Enqueue_ClaimAuditEntry_IsPersistedToDatabase()
        {
            // Arrange
            var entry = new ClaimAuditEntry("claim-1", "POST", DateTime.UtcNow);

            // Act
            _queue.Enqueue(entry);
            await WaitUntilAsync(async () =>
            {
                await using var context = _fixture.CreateContext();
                return await context.ClaimAudits.AnyAsync(a => a.ClaimId == "claim-1");
            });

            // Assert
            await using var verifyContext = _fixture.CreateContext();
            var persisted = await verifyContext.ClaimAudits.SingleAsync(a => a.ClaimId == "claim-1");

            Assert.Equal("POST", persisted.HttpRequestType);
        }

        [Fact]
        public async Task Enqueue_CoverAuditEntry_IsPersistedToDatabase()
        {
            // Arrange
            var entry = new CoverAuditEntry("cover-1", "DELETE", DateTime.UtcNow);

            // Act
            _queue.Enqueue(entry);
            await WaitUntilAsync(async () =>
            {
                await using var context = _fixture.CreateContext();
                return await context.CoverAudits.AnyAsync(a => a.CoverId == "cover-1");
            });

            // Assert
            await using var verifyContext = _fixture.CreateContext();
            var persisted = await verifyContext.CoverAudits.SingleAsync(a => a.CoverId == "cover-1");

            Assert.Equal("DELETE", persisted.HttpRequestType);
        }

        /// <summary>
        /// Polls until the given condition is true or a timeout elapses — needed since the
        /// background service consumes the queue asynchronously, off the test's execution flow.
        /// </summary>
        private static async Task WaitUntilAsync(Func<Task<bool>> condition, int timeoutMs = 5000)
        {
            var elapsed = 0;
            const int interval = 100;

            while (elapsed < timeoutMs)
            {
                if (await condition()) return;
                await Task.Delay(interval);
                elapsed += interval;
            }

            Assert.Fail($"Condition was not met within {timeoutMs}ms.");
        }
    }
}
