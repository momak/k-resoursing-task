using Claims.Auditing.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Claims.Auditing
{
    /// <summary>
    /// Drains <see cref="IAuditQueue"/> and persists audit entries to <see cref="AuditContext"/>
    /// off the HTTP request thread.
    /// </summary>
    public class AuditBackgroundService : BackgroundService
    {
        private readonly IAuditQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AuditBackgroundService> _logger;

        public AuditBackgroundService(
            IAuditQueue queue,
            IServiceScopeFactory scopeFactory,
            ILogger<AuditBackgroundService> logger)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var entry in _queue.DequeueAllAsync(stoppingToken))
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<AuditContext>();

                    switch (entry)
                    {
                        case ClaimAuditEntry claimEntry:
                            context.ClaimAudits.Add(new ClaimAudit
                            {
                                ClaimId = claimEntry.EntityId,
                                HttpRequestType = claimEntry.Action,
                                Created = claimEntry.Timestamp
                            });
                            break;

                        case CoverAuditEntry coverEntry:
                            context.CoverAudits.Add(new CoverAudit
                            {
                                CoverId = coverEntry.EntityId,
                                HttpRequestType = coverEntry.Action,
                                Created = coverEntry.Timestamp
                            });
                            break;
                    }

                    await context.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    // Never let one bad entry kill the consumer loop.
                    _logger.LogError(ex, "Failed to persist audit entry for {EntityId}", entry.EntityId);
                }
            }
        }
    }
}
